#!/usr/bin/env ruby

require 'tools/rake/distrib'

#todo: Make use of options hash in ctor
class Project
	include Rake::TaskUtils
	attr_reader :name, :trunk_folder, :revision, :checkout_url, :checkout_folder
	def initialize(name, checkout_url, checkout_folder, trunk_folder, revision, build_output_folder, lib_folder_hash)
		@name = name
		@checkout_url = checkout_url
		@checkout_folder = checkout_folder
		@revision = revision
		@trunk_folder = normalize(trunk_folder)
		@build_output_folder = build_output_folder
		@lib_folder_of = lib_folder_hash || Hash.new
	end
	def build_output_folder
		normalize(File.join(@trunk_folder, @build_output_folder))
	end
	def lib(project)
	  normalize(File.join(@trunk_folder, @lib_folder_of[project]))
	end
end

###################################
# We are at the following revisions
###################################
REV = {
	:castle => '5888',
	:nhibernate => '4660',
	:fnh => '0e45a91e2501243d0da47ec389ea191a95eec668',
	:spark => 'dd45553925d8ee832e27e84582e120bed8777657',
	:mvccontrib => 'dcc3e2a900409422190c1539404c3e52c4a6da5c',
	:lofus => '47727',
	:tarantino => '121',
	:automapper => '136',
	:json => '32762'
}

#######################################################################
# This global hash of projects defines the library dependency structure
#
#   * project name
#   * trunk folder (svn working folder)
#   * build output folder
#   * hash of lib subfolders per library/project (dependency tree)
#
#######################################################################
PROJECTS = { 
	:castle => Project.new('Castle', 'https://svn.castleproject.org:/svn/castle/trunk', 'C:\dev\oss\castle', 'C:\dev\oss\castle', REV[:castle], 'build/net-3.5/debug', nil),
	:nhibernate => Project.new('NHibernate', 'https://nhibernate.svn.sourceforge.net/svnroot/nhibernate/trunk', 'C:\dev\oss\nhibernate', 'C:\dev\oss\nhibernate', REV[:nhibernate], 'nhibernate/build/NHibernate-3.0.0.Alpha1-debug/bin/net-3.5',
	{
		:castle => 'nhibernate/lib/net/3.5'
	}),
	:nhcontrib => Project.new('NHibernateContrib', 'https://nhcontrib.svn.sourceforge.net/svnroot/nhcontrib/trunk', 'c:\dev\oss\nhcontrib', 'C:\dev\oss\nhcontrib', REV[:nhcontrib], nil, 
	{
		:castle => 'lib/net/2.0',
		:nhibernate => 'lib/net/2.0'
	}),
	:fnh => Project.new('NHibernate.Fluent', '', 'C:\dev\projects\fluent-nhibernate', 'C:\dev\projects\fluent-nhibernate', REV[:fnh], 'build',
	{
		:castle => 'tools/NHibernate',
		:nhibernate => 'tools/NHibernate'
	}),
	:mvc => Project.new('ASP.NET MVC', 'https://maxild-tools.googlecode.com/svn/trunk/MsMvc', 'C:\dev\projects\MsMvc', 'C:\dev\projects\MsMvc\RTM\MVC', 'HEAD', 'bin/debug', nil),
	:mvc2 => Project.new('ASP.NET MVC2', 'https://maxild-tools.googlecode.com/svn/trunk/MsMvc2', 'C:\dev\projects\MsMvc2', 'C:\dev\projects\MsMvc2\P2\MVC', 'HEAD', 'bin/debug', nil),
	:spark => Project.new('Spark', 'todo', 'C:\dev\projects\spark', 'C:\dev\projects\spark', 'todo', '/build/net-3.5.win32-Spark-debug', 
	{
		:mvc => 'bin/aspnetmvc',
		:mvc2 => 'bin/aspnetmvc2'
	}),
	:mvccontrib => Project.new('MvcContrib', '', 'C:\dev\oss\MvcContrib', 'C:\dev\oss\MvcContrib', REV[:mvccontrib], 'build/net-3.5.win32-MVCContrib-debug', 
	{
		:castle => 'bin/castle',
		:mvc => 'bin/AspNetMvc',
		:mvc2 => 'bin/AspNetMvc'
	}),
	:lofus => Project.new('Lofus', 'http://localhost:8081/tfsapp01pr.brfkredit.brf/Application.Laaneberegninger/Head', 'C:\dev\projects\Laaneberegninger', 'C:\dev\projects\Laaneberegninger\Lofus', REV[:lofus], 'bin/debug', nil),
	:tarantino => Project.new('Tarantino', 'http://tarantino.googlecode.com/svn/trunk', 'C:\dev\projects\tarantino', 'C:\dev\projects\tarantino', REV[:tarantino], 'todo', nil),
	:automapper => Project.new('AutoMapper', 'http://automapperhome.googlecode.com/svn/trunk', 'C:\dev\projects\automapper', 'C:\dev\projects\automapper', REV[:automapper], 'build/merge', nil),
	:json => Project.new('Json.NET', 'https://Json.svn.codeplex.com/svn/trunk', 'C:\dev\projects\json', 'C:\dev\projects\json', REV[:json], 'build/debug', nil),
	:totalberegner => Project.new('Totalberegner', 'https://maxild-tools.googlecode.com/svn/trunk/totalberegner', 'c:\dev\projects\totalberegner',  'c:\dev\projects\totalberegner', nil, nil,
	{
		:castle => 'lib/castle',
		:nhibernate => 'lib/nhibernate',
		:fnh => 'lib/fluent.nhibernate',
		:lofus => 'lib/lofus',
		:mvc => 'lib/aspnetmvc',
		:mvc2 => 'lib/aspnetmvc2',
		:spark => 'lib/spark',
		:mvccontrib => 'lib/mvccontrib',
		:automapper => 'lib/automapper',
		:json => 'lib/json',
		:tarantino => 'tools/msbuild.tarantino'
	})
}

NANT_FOLDER = PROJECTS[:totalberegner].trunk_folder + '\tools\nant\bin' # NAnt 0.86 Beta 1

# all lib dependencies are handled in this namespace		
namespace :lib do
	
	desc "SVN Checkout of all projects"
	task :setup => [
		'castle:svncheckout',
		'nh:svncheckout',
		'fnh:svncheckout',
		'mvc:svncheckout',
		'spark:svncheckout',
		'mvccontrib:svncheckout',
		'automapper:svncheckout',
		'json:svncheckout',
		'lofus:svncheckout'
	]
	
	desc "SVN Checkout of all projects (minus Lofus)"
	task :setup_minus_lofus => [
		'castle:svncheckout',
		'nh:svncheckout',
		'fnh:svncheckout',
		'mvc:svncheckout',
		'spark:svncheckout',
		'mvccontrib:svncheckout',
		'automapper:svncheckout',
		'json:svncheckout'
	]
	
	desc "Run all tasks to update Totalberegner lib dependencies"
	task :all => [
		'castle:all', 
		'nh:all',
		'fnh:all', 
		'mvc:all', 
		'spark:all', 
		'mvccontrib:all',
		'automapper:all',
		'json:all',
		'lofus:all'
	]
	
	desc "Run all tasks (minus Lofus) to update Totalberegner lib dependencies"
	task :all_minus_lofus => [
		'castle:all', 
		'nh:all', 
		'fnh:all', 
		'mvc:all', 
		'spark:all', 
		'mvccontrib:all',
		'automapper:all',
		'json:all'
	]
	
	namespace :castle do
		desc "SVN Update, build and distribute Castle"	
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** Castle ******")
		end
		
		desc "SVN Info Castle"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:castle].trunk_folder;
		end
		
		desc "SVN Status Castle"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:castle].trunk_folder;
		end
		
		desc "SVN Checkout Castle"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:castle].revision
			svn.url = PROJECTS[:castle].checkout_url
			svn.folder = PROJECTS[:castle].checkout_folder
		end
		
		desc "SVN Update Castle"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:castle].revision
			svn.working_folder = PROJECTS[:castle].trunk_folder;
		end
		
		desc "Build Castle"
		Rake::NAntTask.new(:build) do |nant|
			nant.tool_path = NANT_FOLDER
			nant.command_line = '-t:net-3.5 -D:common.testrunner.enabled=false -D:assembly.allow-partially-trusted-callers=true'
			nant.working_folder = PROJECTS[:castle].trunk_folder 
		end
		
		desc "Distribute Castle"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:castle].name 
			t.source_folder = PROJECTS[:castle].build_output_folder
			
			t.distrib_to(PROJECTS[:nhibernate].name, PROJECTS[:nhibernate].lib(:castle)) do |nhibernate|
				nhibernate.add('Castle.Core').with_ext('dll', 'pdb', 'xml')
				nhibernate.add('Castle.DynamicProxy2').with_ext('dll', 'pdb', 'xml')
			end
			
			t.distrib_to(PROJECTS[:fnh].name, PROJECTS[:fnh].lib(:castle)) do |fnh|
				fnh.add('Castle.Core').with_ext('dll', 'pdb', 'xml')
				fnh.add('Castle.DynamicProxy2').with_ext('dll', 'pdb', 'xml')
			end
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:castle)) do |totalberegner|
				totalberegner.add('Castle.Core').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Castle.MicroKernel').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Castle.Windsor').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Castle.DynamicProxy2').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Castle.Components.Validator').with_ext('dll', 'pdb', 'xml')
			end
		end
		
		desc "Open Castle in Visual Studio"
		task :ide do
			working_folder = PROJECTS[:castle].trunk_folder;
			cd working_folder do
				sh 'Castle-all-vs2008.sln'
			end	
		end
		
	end
	
	namespace :nh do
		desc "SVN Update, build and distribute NHibernate"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** NHibernate ******")
		end
		
		desc "SVN Info NHibernate"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:nhibernate].trunk_folder
		end
		
		desc "SVN Status NHibernate"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:nhibernate].trunk_folder
		end
		
		desc "SVN Checkout NHibernate"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:nhibernate].revision
			svn.url = PROJECTS[:nhibernate].checkout_url
			svn.folder = PROJECTS[:nhibernate].checkout_folder
		end
		
		desc "SVN Update NHibernate"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:nhibernate].revision
			svn.working_folder = PROJECTS[:nhibernate].trunk_folder
		end
		
		desc "Build NHibernate"
		Rake::NAntTask.new(:build) do |nant|
			nant.tool_path = NANT_FOLDER
			nant.command_line = "-t:net-3.5 -D:skip.tests=true -D:project.config=debug clean build"
			nant.working_folder = "#{PROJECTS[:nhibernate].trunk_folder}\\nhibernate"
		end
		
		desc "Distribute NHibernate"
		Rake::DistribTask.new(:distrib => :schema_distrib) do |t|
			t.source_project = PROJECTS[:nhibernate].name
			t.source_folder = PROJECTS[:nhibernate].build_output_folder
			
			t.distrib_to(PROJECTS[:fnh].name, PROJECTS[:fnh].lib(:nhibernate)) do |fnh|
				fnh.add('NHibernate').with_ext('dll', 'pdb', 'xml')
				fnh.add('Iesi.Collections').with_ext('dll', 'pdb', 'xml')
				fnh.add('log4net').with_ext('dll', 'xml')
				fnh.add('NHibernate.ByteCode.Castle').with_ext('dll', 'pdb', 'xml') 
			end 
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:nhibernate)) do |totalberegner|
				totalberegner.add('NHibernate').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Antlr3.Runtime').with_ext('dll') 
				totalberegner.add('Iesi.Collections').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('log4net').with_ext('dll', 'xml')
				totalberegner.add('NHibernate.ByteCode.Castle').with_ext('dll', 'pdb', 'xml')
			end	
		end
		
		task :schema_distrib do
			FileList["#{PROJECTS[:nhibernate].trunk_folder}\\nhibernate\\src\\**\\*.xsd".gsub(/\\/,'/')].each do |xsd|
				cp xsd, File.join("#{PROJECTS[:totalberegner].lib(:nhibernate)}", xsd.pathmap('%f')).gsub('\\', '/')	
			end
		end
		
		desc "Open NHibernate in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:nhibernate].trunk_folder, 'nhibernate', 'src');
			cd working_folder do
				sh 'NHibernate.Everything.sln'
			end	
		end
		
	end
			
	namespace :fnh do	
		desc "SVN Update, build and distribute fluent.nhibernate"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** Fluent NHibernate ******")
		end				

		desc "SVN Info fluent.nhibernate"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:fnh].trunk_folder
		end
		
		desc "SVN Status fluent.nhibernate"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:fnh].trunk_folder
		end
		
		desc "SVN Checkout fluent.nhibernate"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:fnh].revision
			svn.url = PROJECTS[:fnh].checkout_url
			svn.folder = PROJECTS[:fnh].checkout_folder
		end
		
		desc "SVN Update fluent.nhibernate"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:fnh].revision
			svn.working_folder = PROJECTS[:fnh].trunk_folder
		end
		
		desc "Build fluent.nhibernate"
		Rake::RakeTask.new(:build) do |t|
			t.command_line = 'compile'
			t.working_folder = PROJECTS[:fnh].trunk_folder
		end
		
		desc "Distribute fluent.nhibernate"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:fnh].name
			t.source_folder = PROJECTS[:fnh].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:fnh)) do |totalberegner|
				totalberegner.add('FluentNHibernate').with_ext('dll', 'pdb')
			end
		end	
		
		desc "Open fluent.nhibernate in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:fnh].trunk_folder, 'src');
			cd working_folder do
				sh 'FluentNHibernate.sln'
			end	
		end
		
	end
	
	namespace :mvc do
		desc "Build and distribute ASP.NET MVC"
		task :all => [:msg, :build, :distrib]
	
		task :msg do
			Rake::TaskUtils::flash("****** ASP.NET MVC ******")
		end
		
		desc "SVN Checkout ASP.NET MVC"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:mvc].revision
			svn.url = PROJECTS[:mvc].checkout_url
			svn.folder = PROJECTS[:mvc].checkout_folder
		end
			
		desc "Build ASP.NET MVC"
		Rake::MsBuildTask.new(:build) do |msbuild|
			# OutputPath=#{PROJECTS[:mvc].build_output_folder} avoided because the sh command 'errs' on cygwin
			msbuild.project = 'MvcDev.sln'
			msbuild.targets << 'Clean'
			msbuild.targets << 'Build'
			msbuild.properties['Configuration'] = 'Debug'
			msbuild.working_folder = PROJECTS[:mvc].trunk_folder
		end
	
		desc "Distribute ASP.NET MVC"		
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:mvc].name
			t.source_folder = PROJECTS[:mvc].build_output_folder
			
			t.distrib_to(PROJECTS[:spark].name, PROJECTS[:spark].lib(:mvc)) do |spark|
				spark.add('System.Web.Abstractions').with_ext('dll')
				spark.add('System.Web.Routing').with_ext('dll')
				spark.add('System.Web.Mvc').with_ext('dll', 'pdb')
				spark.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
			
			t.distrib_to(PROJECTS[:mvccontrib].name, PROJECTS[:mvccontrib].lib(:mvc)) do |mvccontrib|
				mvccontrib.add('System.Web.Abstractions').with_ext('dll')
				mvccontrib.add('System.Web.Routing').with_ext('dll')
				mvccontrib.add('System.Web.Mvc').with_ext('dll', 'pdb')
				mvccontrib.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:mvc)) do |totalberegner|
				totalberegner.add('System.Web.Abstractions').with_ext('dll')
				totalberegner.add('System.Web.Routing').with_ext('dll')
				totalberegner.add('System.Web.Mvc').with_ext('dll', 'pdb')
				totalberegner.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
		end
		
		desc "Open ASP.NET MVC in Visual Studio"
		task :ide do
			working_folder = PROJECTS[:mvc].trunk_folder;
			cd working_folder do
				sh 'MvcDev.sln'
			end	
		end
		
	end
	
	namespace :mvc2 do
		desc "Build and distribute ASP.NET MVC2"
		task :all => [:msg, :build, :distrib]
	
		task :msg do
			Rake::TaskUtils::flash("****** ASP.NET MVC2 ******")
		end
		
		desc "SVN Checkout ASP.NET MVC2"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:mvc2].revision
			svn.url = PROJECTS[:mvc2].checkout_url
			svn.folder = PROJECTS[:mvc2].checkout_folder
		end
			
		desc "Build ASP.NET MVC2"
		Rake::MsBuildTask.new(:build) do |msbuild|
			msbuild.project = 'MvcDev.sln'
			msbuild.targets << 'Clean'
			msbuild.targets << 'Build'
			msbuild.properties['Configuration'] = 'Debug'
			msbuild.working_folder = PROJECTS[:mvc2].trunk_folder
		end
	
		desc "Distribute ASP.NET MVC2"		
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:mvc2].name
			t.source_folder = PROJECTS[:mvc2].build_output_folder
			
			t.distrib_to(PROJECTS[:spark].name, PROJECTS[:spark].lib(:mvc2)) do |spark|
				spark.add('System.Web.Mvc').with_ext('dll', 'pdb')
				spark.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
			
			t.distrib_to(PROJECTS[:mvccontrib].name, PROJECTS[:mvccontrib].lib(:mvc2)) do |mvccontrib|
				mvccontrib.add('System.Web.Mvc').with_ext('dll', 'pdb')
				mvccontrib.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:mvc2)) do |totalberegner|
				totalberegner.add('System.Web.Mvc').with_ext('dll', 'pdb')
				totalberegner.add('Microsoft.Web.Mvc').with_ext('dll', 'pdb')
			end
		end
		
		desc "Open ASP.NET MVC2 in Visual Studio"
		task :ide do
			working_folder = PROJECTS[:mvc2].trunk_folder;
			cd working_folder do
				sh 'MvcDev.sln'
			end	
		end
		
	end
	
	namespace :spark do
		desc "SVN Update, build and distribute Spark"
		task :all => [:msg, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** Spark ******")
		end
		
		#todo: incorporate git fetch...
		
		desc "Build Spark"
		Rake::NAntTask.new(:build) do |nant| 
			nant.tool_path = NANT_FOLDER
			nant.command_line = "-t:net-3.5 -D:project.config=debug clean version init commonassemblyinfo compile"
			nant.working_folder = "#{PROJECTS[:spark].trunk_folder}"			
		end
			
		desc "Distribute Spark"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:spark].name
			t.source_folder = PROJECTS[:spark].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:spark)) do |totalberegner|
				totalberegner.add('Spark').with_ext('dll', 'pdb')
				totalberegner.add('Spark.Web.Mvc').with_ext('dll', 'pdb')
				totalberegner.add('Spark.Web.Mvc2').with_ext('dll', 'pdb')
			end
		end
		
		desc "Open Spark in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:spark].trunk_folder, 'src');
			cd working_folder do
				sh 'Spark.sln'
			end	
		end
		
	end
	
	namespace :mvccontrib do
		desc "SVN Update, build and distribute MvcContrib"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** MvcContrib ******")
		end
		
		desc "Build MvcContrib"
		Rake::NAntTask.new(:build) do |nant|
			nant.tool_path = NANT_FOLDER
			nant.command_line = '-buildfile:nant.build -D:project.config=debug clean version init commonassemblyinfo compile move-for-test'	
			nant.working_folder = PROJECTS[:mvccontrib].trunk_folder
		end
		
		desc "Distribute MvcContrib"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:mvccontrib].name
			t.source_folder = PROJECTS[:mvccontrib].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:mvccontrib)) do |totalberegner|
				totalberegner.add('MvcContrib').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('MvcContrib.FluentHtml').with_ext('dll', 'pdb', 'xml')
			end
		end
		
		desc "Open MVCContrib in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:mvccontrib].trunk_folder, 'src');
			cd working_folder do
				sh 'MVCContrib.sln'
			end	
		end
		
	end
	
	namespace :lofus do
		desc "SVN Update, build and distribute Lofus"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** Lofus ******")
		end
		
		desc "SVN Info Lofus"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:lofus].trunk_folder
		end
		
		desc "SVN Status Lofus"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:lofus].trunk_folder
		end
		
		desc "SVN Checkout Lofus"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:lofus].revision
			svn.url = PROJECTS[:lofus].checkout_url
			svn.folder = PROJECTS[:lofus].checkout_folder
		end
		
		desc "SVN Update Lofus"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:lofus].revision
			svn.working_folder = PROJECTS[:lofus].trunk_folder
		end
		
		desc "Build Lofus"
		Rake::MsBuildTask.new(:build) do |msbuild|
			msbuild.project = 'Lofus-vs2008.sln'
			msbuild.targets << 'Clean'
			msbuild.targets << 'Build'
			msbuild.properties['Configuration'] = 'Debug'
			msbuild.properties['OutputPath'] = "#{PROJECTS[:lofus].build_output_folder}"
			msbuild.working_folder = PROJECTS[:lofus].trunk_folder
		end
		
		desc "Distribute Lofus"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:lofus].name
			t.source_folder = PROJECTS[:lofus].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:lofus)) do |totalberegner|
				totalberegner.add('Brf.Lofus.Core').with_ext('dll', 'pdb', 'xml')
				totalberegner.add('Brf.Lofus.Integration').with_ext('dll', 'pdb')
				totalberegner.add('Brf.Lofus.Provider.Common').with_ext('dll', 'pdb')
				totalberegner.add('Brf.Lofus.Provider.Produktkatalog').with_ext('dll', 'pdb')
			end
		end
	end
	
	namespace :automapper do
		
		desc "SVN Update, build and distribute AutoMapper"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** AutoMapper ******")
		end
		
		desc "SVN Info AutoMapper"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:automapper].trunk_folder
		end
		
		desc "SVN Status AutoMapper"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:automapper].trunk_folder
		end
		
		desc "SVN Checkout AutoMapper"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:automapper].revision
			svn.url = PROJECTS[:automapper].checkout_url
			svn.folder = PROJECTS[:automapper].checkout_folder
		end
		
		desc "SVN Update AutoMapper"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:automapper].revision
			svn.working_folder = PROJECTS[:automapper].trunk_folder
		end
		
		desc "Build AutoMapper"
		Rake::NAntTask.new(:build) do |nant|
			nant.tool_path = NANT_FOLDER
			nant.command_line = '-buildfile:AutoMapper.build -t:net-3.5 clean commonassemblyinfo compile merge'
			nant.working_folder = PROJECTS[:automapper].trunk_folder 
		end
		
		desc "Distribute AutoMapper"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:automapper].name
			t.source_folder = PROJECTS[:automapper].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:automapper)) do |totalberegner|
				totalberegner.add('AutoMapper').with_ext('dll', 'pdb')
			end
		end
		
		desc "Open AutoMapper in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:automapper].trunk_folder, 'src');
			cd working_folder do
				sh 'AutoMapper.sln'
			end	
		end
		
	end
	
	namespace :json do
		
		desc "SVN Update, build and distribute Json.NET"
		task :all => [:msg, :svnupdate, :build, :distrib]
		
		task :msg do
			Rake::TaskUtils::flash("****** Json.NET ******")
		end
		
		desc "SVN Info Json.NET"
		Rake::SvnInfoTask.new(:svninfo) do |svn|
			svn.working_folder = PROJECTS[:json].trunk_folder
		end
		
		desc "SVN Status Json.NET"
		Rake::SvnStatusTask.new(:svnstatus) do |svn|
			svn.working_folder = PROJECTS[:json].trunk_folder
		end
		
		desc "SVN Checkout Json.NET"
		Rake::SvnCheckoutTask.new(:svncheckout) do |svn|
			svn.revision = PROJECTS[:json].revision
			svn.url = PROJECTS[:json].checkout_url
			svn.folder = PROJECTS[:json].checkout_folder
		end
		
		desc "SVN Update Json.NET"
		Rake::SvnUpdateTask.new(:svnupdate) do |svn|
			svn.revision = PROJECTS[:json].revision
			svn.working_folder = PROJECTS[:json].trunk_folder
		end
		
		desc "Build Json.NET"
		Rake::MsBuildTask.new(:build) do |msbuild|
			msbuild.project = 'Newtonsoft.Json.sln'
			msbuild.targets << 'Clean'
			msbuild.targets << 'Build'
			msbuild.properties['Configuration'] = 'Debug'
			msbuild.properties['OutputPath'] = "#{PROJECTS[:json].build_output_folder}"
			msbuild.working_folder = File.join(PROJECTS[:json].trunk_folder, 'src')
		end
		
		desc "Distribute Json.NET"
		Rake::DistribTask.new(:distrib) do |t|
			t.source_project = PROJECTS[:json].name
			t.source_folder = PROJECTS[:json].build_output_folder
			
			t.distrib_to(PROJECTS[:totalberegner].name, PROJECTS[:totalberegner].lib(:json)) do |totalberegner|
				totalberegner.add('Newtonsoft.Json').with_ext('dll', 'pdb', 'xml')
			end
		end
	
		desc "Open Json.NET in Visual Studio"
		task :ide do
			working_folder = File.join(PROJECTS[:json].trunk_folder, 'src');
			cd working_folder do
				sh 'Newtonsoft.Json.sln'
			end	
		end
			
	end
	
	namespace :tarantino do
		#todo
	end
	
end # namespace :lib