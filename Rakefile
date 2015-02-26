#!/usr/bin/env ruby

require 'tools/rake/tasks'

ROOT = File.dirname(__FILE__)
COMPANY = 'BRFKredit a/s'
PRODUCT_NS = 'Maxfire'
PRODUCT_DESC = 'Maxfire Webstack Libraries'
PRODUCT_VERSION = '0.1'
COPYRIGHT = 'Copyright 2009 Morten Maxild. All rights reserved.';
CONFIGURATION = 'debug'
SOLUTION = 'Maxfire.sln'

# 3rd party program paths
PATHS = {
	:ncover => "#{File.join(Rake::OSArchitecture.programfiles, 'NCover')}",
	:xunit => File.join(ROOT, 'packages', 'xunit.runners.1.9.2', 'tools')
}

ARCHIVE = {
	:root => 'archive',
	:build => 'archive/build',
	:build_output => 'archive/build/' + CONFIGURATION, # TODO: Copy to build output folder
	:results => 'archive/results'
}
ARCHIVE.each { |k, v| ARCHIVE[k] = File.join(ROOT, v) }

PROP = {
	:version => PRODUCT_VERSION
}

def rf(filename)
	File.join(ARCHIVE[:results], filename)
end

def publish_tool(name, exclude_pattern=nil)
	dest = File.join(ARCHIVE[:tools], name)
	Dir.mkdir dest unless File.exists? dest
	Rake::TaskUtils::cp_wo_svn(File.join(ROOT, 'tools', name), dest, exclude_pattern)
end

namespace :build do

	dev_build = [
		:clean_build,
		:compile,
		#:fxcop,
		#:simian,
		:run_tests,
		#:coverage_report,
		:copy_output_assemblies
	]

	desc "Before Commit Build"
	task :dev => dev_build

	# create archive folders
	task :init do
		Rake::TaskUtils::flash "creating archive folders"
		ARCHIVE.each { |k, v| mkdir_p v unless File.exist?(v) }
	end

	# remove archive folders
	task :clean_all do
		Rake::TaskUtils::flash "Removing all archive folders"
		remove_dir ARCHIVE[:root] if File.exist?(ARCHIVE[:root])
	end

	# remove archive folders, except packages and latestpackage
	task :clean_build do
		Rake::TaskUtils::flash "Removing build archive folders"
		ARCHIVE.each { |k, v| remove_dir v unless !File.exists?(v) || k == :root || k == :latestpackage || k == :packages }
	end

	# Generate CommonAssemblyInfo.cs file
	Rake::AssemblyInfoTask.new(:version) do |asminfo|
		asminfo.company = COMPANY
		asminfo.configuration = CONFIGURATION
		asminfo.copyright = COPYRIGHT
		asminfo.product = PRODUCT_DESC
		build_number = ENV["CCNetLabel"].nil? ? '0' : ENV["CCNetLabel"].to_s
		revision_number = File.exists?('.svn') ? Rake::TaskUtils::svn_revision : '0'
		PROP[:version] = "#{PRODUCT_VERSION}.#{build_number}.#{revision_number}"
		asminfo.version = Rake::Vers.new(PROP[:version])
	end

  desc "verify the version of msbuild"
  Rake::MsBuildTask.new(:verify) do |msbuild|
    # Visual Studio 2013 will exclusively use 2013 MSBuild and C# compilers (assembly version 12.0)
    # and the 2013 Toolset (ToolsVersion 12.0)
    #$version = &"$framework_dir\MSBuild.exe" /nologo /version
    #sh "msbuild /nologo /version"
    #$expectedVersion = "4.0.30319.34209" # This is MSBuild version at framework path
    #expectedVersion = "12.0.31101.0"
    #puts "MSBuild version is $version"
    msbuild.tools_version = '12.0'
    msbuild.target_framework_version = 'v4.5.2'
  end

	desc "Compile all code"
	Rake::MsBuildTask.new(:compile => [:init, :version]) do |msbuild|
		# Visual Studio 2013 (v12.0) uses a ToolsVersion of 12.0.
    # See also http://blogs.msdn.com/b/visualstudio/archive/2013/07/24/msbuild-is-now-part-of-visual-studio.aspx
		msbuild.tools_version = '12.0'
		msbuild.target_framework_version = 'v4.5.2'
		msbuild.project = SOLUTION
		msbuild.targets << 'Clean'
		msbuild.targets << 'Build'
		msbuild.verbosity_level = 'minimal'
		msbuild.properties['Configuration'] = CONFIGURATION
		msbuild.properties['TreatWarningsAsErrors'] = "true"
		msbuild.properties['MvcBuildViews'] = "true"
		msbuild.properties['MvcPublishWebsite'] = "false"
	end

	desc "Perform static code analysis"
	Rake::FxCopTask.new(:fxcop => :compile) do |fxcop|
		fxcop.tool_path = File.join(ROOT, 'tools/fxcop')
		fxcop.rich_console_output = false
		fxcop.assemblies = FileList["src/app/**/bin/#{CONFIGURATION}/#{PRODUCT_NS}.*.dll"]
		fxcop.results_file = rf('FxCopReport.xml')
		fxcop.assembly_search_path = "#{ARCHIVE[:build_output]}"
	end

	desc "Run similarity analyser"
	Rake::SimianTask.new(:simian => :init) do |simian|
		simian_path = File.join(ROOT, 'tools', 'simian')
		simian.tool_path = File.join(simian_path, 'bin')
		simian.results_file = rf('SimianReport.xml')
		simian.stylesheet = File.join(simian_path, 'simian.xsl')
	end

	test_task_names = []
	coverage_results_filenames = []
	 # BUG: We need to distribute NewtonSoft.Json and friends as well
	 # This hack should be replaced with solid code to define the list of build_assemblies
	build_assemblies = []

	FileList["src/test/**/bin/#{CONFIGURATION}/#{PRODUCT_NS}.*.UnitTests.dll"].each do |test_assembly|
		# get the name of the assembly without extension (e.g. Maxfire.Web.Mvc.UnitTests)
		test_assembly_name =  File.basename(test_assembly).ext
		# Find the corresponding production assembly (e.g. Maxfire.Web.Mvc.dll)
		build_assemblies << test_assembly.slice(0, test_assembly.size - '.UnitTests.dll'.size) + '.dll'
		# slice away the Maxfire. prefix and the .UnitTests postfix.
		name = test_assembly_name.slice(PRODUCT_NS.size + 1, test_assembly_name.size - (PRODUCT_NS.size + 1 + '.UnitTests'.size))
		# remove any dots
		name.gsub!(/[\.]/,'')
		# Create the dynamic task name (run_WebMvc_tests)
		task_name = "run_#{name}_tests"
		test_task_names << task_name
		desc "Run tests in #{test_assembly_name} with coverage"
		Rake::XUnitTask.new(task_name => :compile) do |xunit|
			tools_dir = PATHS[:xunit]
			xunit.clr_version = '4'
			xunit.xunit_path = tools_dir
			xunit.test_assembly = test_assembly
			xunit.results_folder = ARCHIVE[:results]
			xunit.test_results_filename = "#{name}UnitTestResults.xml"
			xunit.test_stylesheet = File.join(tools_dir, 'HTML.xslt')
			xunit.calculate_coverage = false
			xunit.coverage_exclude_attrs << 'Maxfire.TestCommons.NoCoverageAttribute'
			xunit.ncover_path = PATHS[:ncover]
			coverage_results_filenames << xunit.coverage_results_filename = "#{name}UnitTestCoverage.xml"
			xunit.coverage_log_filename = "#{name}UnitTestCoverage.log"
			xunit.coverage_assemblies = FileList["#{File.dirname(test_assembly)}/#{PRODUCT_NS}*.dll"].exclude(/.*Tests.dll$/)
		end
	end

	desc "Run all the unit tests"
	task :run_tests => test_task_names

	desc "Create unit test coverage report"
	Rake::NCoverExplorerTask.new(:coverage_report => :run_tests) do |ncover_explorer|
		ncover_explorer.flash_message = "ncoverexplorer: generating unit test coverage report"
		ncover_explorer.tool_path = File.join(ROOT, 'tools/ncover.explorer')
		coverage_results_filenames.each { |coverage_result| ncover_explorer.coverage_files << rf(coverage_result) }
		ncover_explorer.project = PRODUCT_DESC
		ncover_explorer.results_folder = ARCHIVE[:results]
		ncover_explorer.xml_results_filename = 'MaxfireCoverageReport.xml'
		ncover_explorer.html_results_filename = 'MaxfireCoverageReport.html'
	end

	# This 'hacky' technique of copying production assemblies to the build output folder
	# relies on 'run unit tests' task. Each unit test project has to be setup satisfying
	# the following rules:
	#   1) A reference to xunit.dll
	#   2) A reference to the corresponding production assembly under test (i.e A.UnitTests.dll references A.dll).
	task :copy_output_assemblies => :run_tests do
		build_assemblies.each do |src|
			cp src, ARCHIVE[:build_output]
			cp src.ext('pdb'), ARCHIVE[:build_output]
		end
	end

end

namespace :util do

	desc "Force NCover and xUnit.net to run under WOW64 (x86 emulator that allows 32-bit Windows applications to run on 64-bit Windows"
	task :ncover64 do
		ncover_path = Rake::TaskUtils.to_windows_path(File.join(ROOT, 'tools', 'ncover', 'NCover.Console.exe'))
		xunit_path = Rake::TaskUtils.to_windows_path(File.join(PATHS[:xunit], 'xunit.console.exe'))
		working_dir = File.join("#{ENV['ProgramW6432']}", 'Microsoft SDKs', 'Windows', 'v7.0', 'bin')
		cd working_dir do
			sh "CorFlags.exe #{ncover_path} /32BIT+"
			sh "CorFlags.exe #{xunit_path} /32BIT+"
		end
	end

	desc "Start Visual Studio"
	task :ide do
		cd ROOT do
			sh SOLUTION
		end
	end

	desc "Open Windows Explorer with focus on totalberegner folder"
	task :explorer do
		sh 'start explorer /e,c:\dev\projects,/Select,c:\dev\projects\maxfire'
	end

end
