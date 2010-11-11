Handling ambiguous matches

StorEvil will choose the context member that has been most recently used
in the case of ambiguous matches.

This allows you to "scope" your context classes.

Scenario: An ambiguous match

	# since this is a method on FooContext...
	using foo context
	# when this line is interpreted, StorEvl will choose the method on FooContext
	call an ambiguous method
	method on foo should have been called

Scenario: An ambiguous match where both are called

	using foo context
	# since this is a method on BarContext...
	using bar context
	# when this line is interpreted, StorEvl will choose the method on BarContext
	call an ambiguous method
	method on bar should have been called
