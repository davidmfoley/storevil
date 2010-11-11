StorEvil has various different ways you can use tabular data to 
drive a scenario. It supports mapping tables of data to parameters
on your context methods depending on the type of parameter.

You can map tabular data to:
- an array of arrays of any basic type that is supported by StorEvil 
	- (int, decimal, string, as well as enum types)
- an array of any type with public properties and fields
	- top row of the table is used to determine the names of the fields

You can also map name/value pairs (in a table with two columns) to:
- A single instance of any type
- A hashtable


Scenario Outline: An example is a parameterized scenario that is run once for each example row

	When team has a record of <wins> wins, <losses> losses and <draws> draws
	Their point total should be <expected>
	
	Examples:
	|wins|losses|draws|expected|
	| 0  | 0    | 0   | 0      |
	| 1  | 0    | 0   | 3      |
	| 0  | 10   | 0   | 0      |
	| 0  | 0    | 1   | 1      |
	| 15 | 4    | 9   | 54     |

Scenario: A table of data can map to a string[][]

	Given the following competition groups:
	| South Africa| Mexico    | Uruguay     | France   |
	| Argentina   | Nigeria   | South Korea | Greece   |
	| USA         | Algeria   | England     | Slovenia |
	| Germany     | Australia | Serbia      | Ghana    |
	
	Then there should be 4 groups
	And USA and England should be in the same group

Scenario: A table of data that maps to an array of types

	Given the following teams:
	| Rank | Nation      | Region       | 
	| 1    | Spain       | Europe       | 
	| 2    | Brazil      | SouthAmerica | 
	| 3    | Netherlands | Europe       | 
	| 4    | Italy       | Europe       | 
	
	Then there should be 4 teams
	and Spain should be ranked 1
	and Italy should be in Europe
	and Brazil should be ranked 2

Scenario: Setting attributes of a single instance of any type

	Given the following team:
	| Rank	 | 18           |
 	| Nation | USA          |
	| Region | NorthAmerica |
	
	And the following team:
	| Rank	 | 39           |
 	| Nation | Ireland      |
	| Region | Europe       |

	Then there should be 2 teams
	and USA should be ranked 18
	and Ireland should be in Europe

Scenario: Using a hash table 

	Given the following roster:
	| 7  | Beasley          |
 	| 12 | Altidore         |
 	| 1  | Howard           |
	
	Then there should be 3 players
	and Beasley should be number 7
	and Altidore should be number 12
	and Howard should be number 1

