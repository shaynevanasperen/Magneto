param(
	$prereleaseVersion = (property prereleaseVersion "beta-1"),
	$ignoreNugetPushErrors = (property ignoreNugetPushErrors "The underlying connection was closed: An unexpected error occurred on a receive.;exists in compilation with different binary hash") #SymbolSource duplicate package error
)

Include-PluginScripts

task . Clean, Version, Compile, Test, Pack, Push