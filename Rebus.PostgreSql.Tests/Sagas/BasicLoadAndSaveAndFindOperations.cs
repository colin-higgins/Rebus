﻿using NUnit.Framework;
using Rebus.Tests.Contracts.Sagas;

namespace Rebus.PostgreSql.Tests.Sagas
{
    [TestFixture]
    public class BasicLoadAndSaveAndFindOperations : BasicLoadAndSaveAndFindOperations<PostgreSqlSagaStorageFactory> { }
}