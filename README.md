# D2L.Services.Core.Postgres

This is a library that wraps Npgsql. It provides a simpler and easier way to interface with a Postgres database and adds additional functionality.

### Warning

Not all features of this library have been fully tested. Wait for version 1.0.0 to be completed before you depend on this.

### TODO:

**Version 1.0**

* Automated tests
* Document bugs and unexpected behaviour in Npgsql 3.0

**Version 1.1**

* Bring back support for reading a result set one row at a time (ExecReadOnline[Async])

**Version 1.2**

* Support for type converters
* Support for adding IEnumerable parameters (not just Array and IList)

**Version 2.0**

* Upgrade to Npgsql 3.1
    * Fixes bug with multi-statement commands
    * Fixes bugs with the connection pool, improving performance
    * Connections will actually be openned fully asynchronously when using OpenAsync
    * Transactions may be committed and rolled back asynchronously
    * Connection string parameter `Connection Lifetime` renamed to `Connection Idle Lifetime`
    * Breaking changes to exception handling

**Version 2.1**

* Support transaction savepoints
* If possible, allow transactions to be marked as readonly/deferrable
