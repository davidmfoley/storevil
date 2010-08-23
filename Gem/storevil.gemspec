version = File.read(File.expand_path("../VERSION", __FILE__)).strip

Gem::Specification.new do |spec|
	spec.platform    = Gem::Platform::RUBY
	spec.name        = "storevil"
	spec.version     = version + "." + Time.now.strftime("%Y%m%d")
	spec.files 		 = Dir['lib/**/*']
	spec.summary     = "StorEvil is a natural language BDD framework for .NET."
	spec.description = "StorEvil is a BDD tool for .NET that you can use to write specifications in English, and then execute those specifications. It parses the text using conventions and removes the need to write regular expressions in most cases. It supports .NET languages such as C#."
	spec.author     = "David M. Foley"
	spec.email       = "storevil@googlegroups.com"
	spec.homepage    = "http://github.com/davidmfoley/storevil"
	spec.rubyforge_project = "storevil"	
	spec.add_dependency('sparkviewengine','> 1.0.0.0')
end