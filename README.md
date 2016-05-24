# D2L.Services.Core.Postgres

This is a library that wraps Npgsql. It provides a simpler and easier way to interface with a Postgres database and adds additional functionality.

### Warning

Not all features of this library have been fully tested. Wait for version 1.0.0 to be completed before you depend on this.

### Publishing a New Version

The D2L.Services.Core.Postgres library is automatically packaged and published upon a merge to master if the version number in **AssemblyInfo.cs** differs from the latest published version. (May take up to 5 minutes to trigger. Jenkins job can be found [here](http://prod.build.d2l/job/Dev/job/D2L.Services.Core/job/D2L.Services.Core.Postgres/job/BuildTestAndPublish/).)

To package the library on your local machine for testing purposes, run **package.ps1** in the **D2L.Services.Core.Postgres** folder.

### TODO:

**Version 1.0**

* Automated tests

**Version 1.1**

* Bring back support for reading a result set one row at a time (ExecReadOnline[Async])

**Version 1.2**

* Support for type converters
* Support for adding IEnumerable parameters (not just Array and IList)

**Version 1.3**

* Support transaction savepoints
* If possible, allow transactions to be marked as readonly/deferrable
