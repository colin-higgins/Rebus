﻿using System;
using Rebus.Logging;
using Rebus.Pipeline;
using Rebus.Threading;
using Rebus.Transport;

namespace Rebus.Workers.ThreadBased
{
    /// <summary>
    /// Implementation of <see cref="IWorkerFactory"/> that creates <see cref="ThreadWorker"/> instances when asked for
    /// an <see cref="IWorker"/>. Each <see cref="ThreadWorker"/> has its own dedicated worker thread that performs
    /// all the work (which consists of receiving new messages and running continuations)
    /// </summary>
    public class ThreadWorkerFactory : IWorkerFactory
    {
        readonly ThreadWorkerSynchronizationContext _threadWorkerSynchronizationContext = new ThreadWorkerSynchronizationContext();
        readonly ParallelOperationsManager _parallelOperationsManager;
        readonly ITransport _transport;
        readonly IPipeline _pipeline;
        readonly IPipelineInvoker _pipelineInvoker;
        readonly IBackoffStrategy _backoffStrategy;
        readonly IRebusLoggerFactory _rebusLoggerFactory;
        readonly TimeSpan _workerShutdownTimeout;

        /// <summary>
        /// Constructs the worker factory
        /// </summary>
        public ThreadWorkerFactory(ITransport transport, IPipeline pipeline, IPipelineInvoker pipelineInvoker, int maxParallelism, IBackoffStrategy backoffStrategy, IRebusLoggerFactory rebusLoggerFactory, TimeSpan workerShutdownTimeout)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            if (pipelineInvoker == null) throw new ArgumentNullException(nameof(pipelineInvoker));
            if (backoffStrategy == null) throw new ArgumentNullException(nameof(backoffStrategy));
            if (rebusLoggerFactory == null) throw new ArgumentNullException(nameof(rebusLoggerFactory));
            if (maxParallelism <= 0) throw new ArgumentOutOfRangeException($"Cannot use value '{maxParallelism}' as max parallelism!");
            if (workerShutdownTimeout == null) throw new ArgumentNullException(nameof(workerShutdownTimeout));

            _transport = transport;
            _pipeline = pipeline;
            _pipelineInvoker = pipelineInvoker;
            _backoffStrategy = backoffStrategy;
            _rebusLoggerFactory = rebusLoggerFactory;
            _parallelOperationsManager = new ParallelOperationsManager(maxParallelism);
            _workerShutdownTimeout = workerShutdownTimeout;
        }

        /// <summary>
        /// Creates a new worker (i.e. a new thread) with the given name
        /// </summary>
        public IWorker CreateWorker(string workerName)
        {
            return new ThreadWorker(
                _transport,
                _pipeline,
                _pipelineInvoker,
                workerName,
                _threadWorkerSynchronizationContext,
                _parallelOperationsManager,
                _backoffStrategy,
                _rebusLoggerFactory,
                _workerShutdownTimeout);
        }
    }
}