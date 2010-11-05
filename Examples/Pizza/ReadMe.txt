This is an example set of StorEvil specifications for an imaginary Pizza business.
 
In order to execute the specs, make sure you have StorEvil built and in your path.

Once that is built, you can run the following command in this folder:
  storevil execute 

... which will run all of the specs and output the results to the console. 

It will use the settings in the storevil.config file, as long as you are in this folder or a sub folder... 
StorEvil searches up from the current directory until it finds a storevil.config file. 
(So you can navigate to a subfolder and execute only the specs in that folder).

You can also use StorEvil to generate NUnit fixtures:
  storevil nunit Generated.cs
  
... this will generate NUnit fixtures in the file Generated.cs, in this folder.

You can also generate an HTML report with the spec results by using either of the following (equivalent) commands:

Long form:
  storevil execute --output-file pizza.storevil.output.html --output-file-format spark --output-file-template default.spark

Short form:
  storevil execute -o pizza.storevil.output.html -f spark -t default.spark

This example includes a failing and a pending scenario, to show the behavior of StorEvil in each case.
