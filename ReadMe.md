This project is an implementation for accessing and using a remote Test Harness server from multiple concurrent clients. The Test Harness will retrieve test code from a Repository server.

One or more client(s) will concurrently supply the Test Harness with Test Requests.

The Test Harness will request test drivers and code to test, as cited in a Test Request, from the Repository.

One or more client(s) will concurrently extract Test Results and logs by enqueuing requests and displaying Test Harness and/or Repository replies when they arrive.

The TestHarness, Repository, and Clients are all distinct projects that compile to separate executables. All communication between these processes will be based on message-passing Windows Communication Foundation (WCF) channels.

Client activities will be defined by user actions in a Windows Presentation Foundation (WPF) user interface. 


To run the project:
--> open command prompt in admin mode and make sure that devenv path is set up. 
--> run compile.bat from the command prompt 
--> run run.bat from the command prompt.