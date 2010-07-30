StorEvil will choose the context member that has been most recently used
in the case of ambiguous matches

Scenario: An ambiguous match
using foo context
call an ambiguous method
method on foo should have been called

Scenario: An ambiguous match where both are called
using foo context
using bar context
call an ambiguous method
method on bar should have been called
