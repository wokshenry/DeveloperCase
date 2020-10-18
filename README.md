# DeveloperCase
Creating the database Using A script file.

The database schema can be regenerated from the attached Sales script file that is in the database folder through the following process: -

I.	Open SQL Server Management Studio as an Administrator. 
II.	Navigate the tree view to File -> Open -> File. This will display a dialogue box where you will navigate to the script folder.
III.	Select the file by either double clicking on it or single clicking and then click on the select button
IV.	Once the script is open then Click on the Execute button. 
 

Open and Running the Application.
This application was developed using ASP.NET CORE MVC which can be opened by Visual Studio 2019. This system will be in the folder called DeveloperCase.
The application can be opened using two methods: -
I.	From the solution folder.
II.	From Visual Studio 2019
Opening From the solution Folder.
 Right click on a file called DeveloperCase.sln that is in the DeveloperCase folder to display a dialogue box. On the dialogue box, select Open with to display another mini dialogue box then select Visual Studio 2019 as shown below.
 

Opening from Visual Studio 2019.
An application can be open from visual studio using the following steps: -
I.	Start visual studio instance on your computer.
II.	On your right-hand side select Open a project / solution to display a file explorer dialogue.
III.	Navigate to the application solution file and select DeveloperCase.sln file.
IV.	Select open to open the application.

Configuring the connection string.
Once the application has loaded in visual studio 2019, you have to change the connection string to point to your database and this can be achieved through the following steps:-
I.	Open the solution explore and look for a json file called appsettings.json and double click on it to open.
II.	Replace the Server value, Id=value and Password value that located in the DefaultConnection with your server credentials as shown below.
 
Running the application
The application can be run by clicking on the IIS Express button as show below.
Alternatively, the hosted version of this hosted system can be accessed from this link :- http://52.188.137.38:8080/sales 
