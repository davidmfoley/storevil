A starter storevil.config file and some examples have been written to this directory. Check them out.

First steps to getting a working storevil project:

1. Open the storevil.config file and read through its contents. There are a couple of things to set in there.
2. Add a reference to StorEvil.Core to your C# project.
3. Add the file ExampleContext.cs to your C# project
4. Execute the following command line to run storevil:
	storevil execute
	
Once this is working, you can try:

- Opening the file example.feature and having a look at the syntax.
	You can add or modify the scenarios and check the results

- Checking out an html report of the storevil results:
	storevil execute -o storevil.output.html
	
- Making the failing & pending examples pass by adding methods to ExampleContext.cs