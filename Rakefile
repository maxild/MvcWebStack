#!/usr/bin/env ruby

require 'tools/rake/tasks'
#require 'find'
#require 'zip/zip'
#require 'zip/zipfilesystem'

ROOT = File.dirname(__FILE__)
COMPANY = 'BRFKredit a/s'
PRODUCT_NS = 'Maxfire'
PRODUCT_DESC = 'Maxfire Commons Library'
PRODUCT_VERSION = '0.1'
COPYRIGHT = 'Copyright 2009 Morten Maxild. All rights reserved.';
CONFIGURATION = 'debug'
SOLUTION = 'Maxfire.sln'

ARCHIVE = {
	:root => 'archive',
	:build => 'archive/build',
	:build_output => 'archive/build/' + CONFIGURATION,
	:results => 'archive/results',
	:publish => 'archive/publish',
	:tools => 'archive/publish/tools',
	:latestpackage => 'archive/latestpackage',
	:packages => 'archive/packages'
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
		:fxcop, 
		:simian, 
		:run_unit_tests, 
		:unit_test_coverage_report,
		:coverage_report
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
		asminfo.version = Rake::Version.new(PROP[:version])
	end
	
	desc "Compile all code"
	Rake::MsBuildTask.new(:compile => [:init, :version]) do |msbuild|
		msbuild.project = File.join('src', SOLUTION)
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
		fxcop.assemblies = FileList["src/app/bin/**/#{PRODUCT_NS}.*.dll"]
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
	
	desc "Run unit tests with coverage"
	Rake::XUnitTask.new(:run_unit_tests => :compile) do |xunit|
		# todo: make test_tool_path and coverage_tool_path
		# todo: Make api like xunit.test.tool_path and xunit.coverage.tool_path etc..
		# todo: make UnitTestRunner independent of NCoverTask
		xunit.tool_path = File.join(ROOT, 'tools')
		xunit.test_assembly = FileList["src/test/UnitTests/**/*.UnitTests.dll"]
		xunit.results_folder = ARCHIVE[:results]
		xunit.test_results_filename = 'UnitTestResults.xml'
		xunit.test_stylesheet = File.join(ROOT, 'tools', 'xunit', 'xUnitSummary.xsl')
		xunit.calculate_coverage = true
		xunit.coverage_results_filename = 'UnitTestCoverage.xml'
		xunit.coverage_log_filename = 'UnitTestCoverage.log'
		xunit.coverage_assemblies = FileList["src/test/UnitTests/**/#{PRODUCT_NS}*.dll"].exclude(/.*Tests.dll$/)
	end
	
	Rake::NCoverExplorerTask.new(:merge_coverage => [:run_unit_tests, :run_integration_tests]) do |ncover_explorer|
		ncover_explorer.flash_message = "ncoverexplorer: merging coverage files"
		ncover_explorer.tool_path = File.join(ROOT, 'tools/ncover.explorer')
		ncover_explorer.coverage_files << rf('UnitTestCoverage.xml')
		ncover_explorer.coverage_files << rf('IntegrationTestCoverage.xml')
		ncover_explorer.project = PRODUCT_DESC
		ncover_explorer.results_folder = ARCHIVE[:results]
		ncover_explorer.merged_results_filename = 'Coverage.xml'
	end
	
	desc "Create unit test coverage report"
	Rake::NCoverExplorerTask.new(:unit_test_coverage_report => :run_unit_tests) do |ncover_explorer|
		ncover_explorer.flash_message = "ncoverexplorer: generating unit test coverage report"
		ncover_explorer.tool_path = File.join(ROOT, 'tools/ncover.explorer')
		ncover_explorer.coverage_files << rf('UnitTestCoverage.xml')
		ncover_explorer.project = PRODUCT_DESC
		ncover_explorer.results_folder = ARCHIVE[:results]
		ncover_explorer.xml_results_filename = 'UnitTestCoverageReport.xml'
		ncover_explorer.html_results_filename = 'UnitTestCoverageReport.html'
	end
	
	desc "Create overall coverage report"
	Rake::NCoverExplorerTask.new(:coverage_report => :merge_coverage) do |ncover_explorer|
		ncover_explorer.flash_message = "ncoverexplorer: generating coverage report"
		ncover_explorer.tool_path = File.join(ROOT, 'tools/ncover.explorer')
		ncover_explorer.coverage_files << rf('Coverage.xml')
		ncover_explorer.project = PRODUCT_DESC
		ncover_explorer.results_folder = ARCHIVE[:results]
		ncover_explorer.xml_results_filename = 'CoverageReport.xml'
		ncover_explorer.html_results_filename = 'CoverageReport.html'
	end
	
end

namespace :util do

	desc "Start Visual Studio"
	task :ide do
		working_folder = File.join(ROOT, 'src')
		cd working_folder do
			sh SOLUTION
		end	
	end
	
	desc "Open Windows Explorer with focus on totalberegner folder"
	task :explorer do
		sh 'start explorer /e,c:\dev\projects,/Select,c:\dev\projects\maxfire'
	end
			
end