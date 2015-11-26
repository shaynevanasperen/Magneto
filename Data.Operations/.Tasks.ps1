param(
	$prereleaseVersion = (property prereleaseVersion "beta-1"),
	$ignoreNugetPushErrors = (property ignoreNugetPushErrors "exists in compilation with different binary hash") #SymbolSource duplicate package error
)

Include-PluginScripts

task . Clean, Version, Compile, Test, Pack, Push