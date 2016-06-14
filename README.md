# D2L.Services.Core.Postgres

This is a library that wraps Npgsql. It provides a simpler and easier way to interface with a Postgres database and
adds additional functionality.

### Running Integration Tests

Before running the integration tests, you must build the test database. To do this, run the SetupTestDatabase.ps1
script in the testdb directory. If your Postgres bin directory is not `C:\Program Files\PostgreSQL\9.5\bin`, then you
will need to pass the path of your Postgres bin directory as an argument to the script.

### Publishing a New Version

The D2L.Services.Core.Postgres library is automatically packaged and published upon a merge to master if the version
number in **AssemblyInfo.cs** differs from the latest published version. (May take up to 5 minutes to trigger.
Jenkins job can be found
[here](http://prod.build.d2l/job/Dev/job/D2L.Services.Core/job/D2L.Services.Core.Postgres/job/BuildTestAndPublish/).)

To package the library on your local machine for testing purposes, run **LocalBuildAndPackage.ps1** in the **build** folder.

### TODO:

**Version 1.1**

* Support for automatic type converters

**Version ??? (1.2?)**

* Bring back support for reading a result set one row at a time (ExecReadOnline[Async])

**Possible Future Work**

Add support for transaction savepoints
