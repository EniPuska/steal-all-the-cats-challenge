# steal-all-the-cats-challenge

- The application is created with .net8 the startup project is StealCatApi 

- I use IISExpress to run my webApi the version of my IIS Express is 10.0

- I use Sql Server 16.0

- On webApi and at the launchSettings.json are the necessary application url for iis express so when we run the program to open the swagger index page automatically

- The settings of the database are on StealCatApi project and on the file named appsettings.json there i have add my server name and the name of the database.

I used for Authentication -> Windows Authentication Type.

For the initial migration of the Database we need to open a terminal on StealCat.Data and run the following command -> dotnet ef migrations add InitialSetup

After that when you run the program for the first time it will create the necessary database that we need to store our data.





