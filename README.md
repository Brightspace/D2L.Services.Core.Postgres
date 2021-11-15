# D2L.Services.Core.Postgres

This is a library that wraps Npgsql. It provides a simpler and easier way to interface with a Postgres database and
adds additional functionality. A version roadmap and full documentation can be found at the
[docs.dev page](http://docs.dev.d2l/index.php/D2L.Services.Core.Postgres)

### Running Integration Tests

Before running the integration tests, you must build the test database. To do this, run the SetupTestDatabase.ps1
script in the testdb directory. If your Postgres bin directory is not `C:\Program Files\PostgreSQL\12\bin`, then you
will need to pass the path of your Postgres bin directory as an argument to the script.

### Generating or Updating the Documentation

* Install Sandcastle Help File Builder (Download [here](https://github.com/EWSoftware/SHFB/releases))
* Build the solution
* Open the Sandcastle project file (D2L.Services.Core.Postgres.shfbproj)
* Under the **Help File** section, update the **Help file version** to match the new package version
* If you've updated to a new version of Npgsql, update the hint path of the Npgsql reference
* Save your changes and build the documentation file
* Upon publishing a new version of D2L.Services.Core.Postgres, upload the updated documentation file to docs.dev
[here](http://docs.dev.d2l/index.php/File:D2L.Services.Core.Postgres.chm)

### Publishing a New Version

The D2L.Services.Core.Postgres library is automatically packaged and published upon a merge to master if the version
number in **AssemblyInfo.cs** differs from the latest published version. (May take up to 5 minutes to trigger.
Jenkins job can be found
[here](http://prod.build.d2l/job/Dev/job/D2L.Services.Core/job/D2L.Services.Core.Postgres/job/BuildTestAndPublish/).)

To package the library on your local machine for testing purposes, run **LocalBuildAndPackage.ps1** in the **build** folder.
