Story: Tic Tac Toe Rules

Scenario: X is first player
Given a new game
Then the current player should be X

Scenario: Game state, move by move
	Given a new game
	When X plays in the top left
	Then the current player should be O
	And the board state should be
	|X| | |
	| | | |
	| | | |
	When O plays in the bottom right
	Then the board state should be 
	|X| | |
	| | | |
	| | |O|

Scenario: O wins
	Given the following board:
	|X|X|O|
	|X|O| | 
	| | | |
	When O plays in the bottom left
	Then the winner should be O


Scenario: X wins
	Given the following board:
	|X|X|O|
	|X|X|O| 
	|O|O| |
	When X plays in the bottom right
	Then the winner should be X
	
Scenario: Cat's game
	Given the following board:
	|X|O|X|
	|O|O|X|
	|X|X|O|
	Then it should be a cat's game

Scenario: horizontal win
	Given the following board:
	|X|X|X|
	|X|O|O| 
	|O|O|X|
	Then the winner should be X
	
Scenario: horizontal win in middle
	Given the following board:	
	|X|O|O|
	|X|X|X| 
	|O|O|X|
	Then the winner should be X
	
Scenario: horizontal win in bottom
	Given the following board:	
	|X|X| |
	|X|O|X| 
	|O|O|O|
	Then the winner should be O

Scenario: vertical win in middle
	Given the following board:	
	|X|X|O|
	|O|X|O| 
	|O|X|X|
	Then the winner should be X
	
Scenario: vertical win in right
	Given the following board:	
	|X|O|X|
	|O|O|X| 
	|O|X|X|
	Then the winner should be X
	
Scenario: vertical win in left
	Given the following board:	
	|X|O|O|
	|X|O|X| 
	|X|X|O|
	Then the winner should be X
	
Scenario: diagonal win
	Given the following board:	
	|X|O|O|
	|X|O|X| 
	|O|X| |
	Then the winner should be O