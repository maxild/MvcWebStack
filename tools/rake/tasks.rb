#!/usr/bin/env ruby

require 'rake'
require 'rake/tasklib'
require 'erb'

module Rake
	
	module TaskUtils
		
		def self.svn_revision(working_folder=nil)
			if working_folder
				cd working_folder do
					cout = `svn.exe info`
				end
			else
				cout = `svn.exe info`
			end
			cout =~ /Revision: (\d+)/
			$1	
		end
		
		# Copy +src+ to +dest+, where the basename of +src+ (i.e. the last folder
		# in the path) is created as a subfolder of +dest+, and all content of
		# +src+ is copied recursively to +dest+. The important thing is that an 
		# .svn subfolders are not copied, and therefore does the copy work like 
		# an svn export if +src+ is an svn working folder.  
		def self.cp_wo_svn(src, dest, exclude_pattern=nil)
			fail "src '#{src}' does not exist" unless File.exists? src
			fail "dest '#{dest}' does not exist" unless File.exists? dest
			fail "src must be a directory" unless File.directory?(src)
			fail "dest must be a directory" unless File.directory?(dest)
			excl_regex = Regexp.new(exclude_pattern) if exclude_pattern
			Dir.foreach(src) do |f|
        		next if f == "." || f == ".." || f == ".svn"
    			current_src = File.join(src, f)
    			current_dest = File.join(dest, f)
        		is_dir = File.directory?(current_src)
    			if is_dir
    				Dir.mkdir current_dest unless File.exists? current_dest
  				else
  					copy_file current_src, current_dest unless excl_regex and excl_regex.match(f)
				end
        		cp_wo_svn(current_src, current_dest, exclude_pattern) if is_dir 
    		end
    	end
	    
	    def cp_wo_svn(src, dest)
	    	TaskUtils::cp_wo_svn(src, dest)
	  	end
	  	
		def self.flash(msg)
			m = msg.upcase
			line = "=" * m.length 
			puts "\n" + line
			puts m
			puts line + "\n"
		end
		
		def flash(msg)
			TaskUtils::flash(msg)
		end
		
		# both win32 and cygwin can live with a normalized path
		def self.normalize(path)
	    	path.gsub(/\\/, '/')
	  	end
	    
	  	def normalize(path)
	  		TaskUtils::normalize(path)
		end
	  	protected :normalize
	    
	  	def self.to_windows_path(path)
	  		normalize(path).gsub(/\//, '\\')
		end
		
	    def nil_or_empty?(x)
			x.nil? || x.empty?
		end
		protected :nil_or_empty?
	
		# hack to find out if we are running inside IronRuby
		# could also check if load_assembly method is supported 
		def self.ir?
			begin
				require 'mscorlib'
				ir = true			
			rescue LoadError
				ir = false
			end
			ir
		end
		
		def self.require_xml
			if ir?
				# IronRuby: Use .NET Framework to work with XML
				require 'mscorlib'
				require 'System'
				require 'System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'		
			else
				# MRI: Use ruby-libxml and ruby-libxslt gems to work with XML
				require 'xml'
				require 'libxslt'
			end
		end
		
		def self.execute_in(&block)
			# yield does not work in IronRuby. Therefore instead of
			# returning the return of yield (which ruby will do 
			# automatically), we use the yielded value to carry 
			# the result in a return_value attribute (not pretty!)
			ruby = RubyInterpreter.new(ir?)
			result = yield ruby if block_given?
			ruby.return_value
		end
		
		def execute_in(&block)
			TaskUtils::execute_in(&block)		
		end
		
		def first(x)
			if (! x.nil?) && x.is_a?(Array)
				x.first
			else
				x
			end
		end
		protected :first
		
	end
	
	class RubyInterpreter
		attr_accessor :return_value # hack, because IronRuby has a bug
		def initialize(ir)
			@ironruby = ir		
		end
		# IronRuby
		def net
			yield if @ironruby
		end
		# MRI
		def mri 
			yield unless @ironruby
		end
	end
	
	class XPathDoc
		include TaskUtils
		
		def initialize(file)
			TaskUtils::require_xml
			execute_in do |ruby|
				ruby.net { 
					xpath_doc = System::Xml::XPath::XPathDocument.new(file)
					@xpath_nav = xpath_doc.create_navigator
				}
				ruby.mri { @doc = LibXML::XML::Document.file(file) }
			end		
		end 
		
		def find(xpath)
			execute_in do |ruby|
				# hack because IronRuby has a bug
				ruby.net { ruby.return_value = @xpath_nav.evaluate(xpath).to_s }
				ruby.mri { ruby.return_value = @doc.find(xpath).to_s }
			end
		end
	end

	class XslTransform
		# I have let go of the SRP, because if I perform the transform
		# and the file saving in different scopes, I am having trouble
		# saving the ClrString (or the String, after to_s) to a file.
		def self.save(file, options)
			xml_file = options[:xml]
			xslt_file = options[:xslt]
			TaskUtils::require_xml
			TaskUtils::execute_in do |ruby|
				ruby.net {
					xslt = System::Xml::Xsl::XslCompiledTransform.new;
					xslt.load(xslt_file);
					xslt.transform(xml_file, file)
				}	
				ruby.mri {
					stylesheet_doc = LibXML::XML::Document.file(xslt_file)
  					stylesheet = LibXSLT::XSLT::Stylesheet.new(stylesheet_doc)
  					xml_doc = XML::Document.file(xml_file)
  					html = stylesheet.apply(xml_doc)
  					File.open(file, 'w') do |f|
						f.puts html
					end
				}
			end	
		end 
	end
	
	###########################################################################
	# Note: Lidt underligt med at Rake::XXXTask.new(:some_task) do |t| ... end 
	# ikke indeholder den action-block som task benytter. F.eks. kan man ikke 
	# skrive til stdout i denne block, der jo er der for at ctor kan indsamle 
	# input til at sende videre til definition af action-blocken. Med andre 
	# ord er action-block skjult for den der skriver Rakefile'n.
	###########################################################################
	class TaskBase < TaskLib
		include TaskUtils
		
		def initialize(*args) 
			init
			yield self if block_given?
      define(*args)
		end
		
		# to be overriden by subclass
		def init
		end
		protected :init
		
		def before_execute
		end
		protected :before_execute
		
		# to be overriden by subclass
		def execute	
		end
		protected :execute
		
		def after_execute
		end
		protected :after_execute
		
		def define(*args)
			block = lambda { 
				before_execute
				execute
				after_execute
			} 
      Rake::Task.define_task(*args, &block)
      self
		end
		private :define
	end
	
	class ToolTask < TaskBase
		
		attr_accessor :tool_path, :tool_name, :working_folder, :command_line
		
		class << self
			# A derived class that override tool_args 
			# should call this to make tool_name readonly,
			# and hide the command_line attribute.
			def remove_tool_attr
				private :tool_name=, :command_line=, :command_line
			end
		end
		
		# construct the full path to the command-line tool
		def tool_exe
			raise "No tool_name passed to task" if nil_or_empty? tool_name
			return tool_name if nil_or_empty? tool_path
			normalize(File.join(tool_path, tool_name))
		end
		protected :tool_exe
		
		# construct the command-line args
		def tool_args
			command_line || ''
		end
		protected :tool_args
		
		def command
			"#{tool_exe} #{tool_args}"
		end
		protected :command
		
		def init
			@working_folder = '.'
			@tool_path = ''
			@tool_name = ''
		end
		
		# define a normal task with a shell action
		def execute
			cd working_folder do
				cmd = command 
				puts cmd
				sh cmd do |ok, status|	
					@exit_code = status.exitstatus unless ok
				end
			end
		end
		
		def exit_code
			@exit_code || 0
		end
		protected :exit_code
	end
	
	class Version
		attr_accessor :major, :minor, :build, :revision
		def initialize(version=nil)
			@major = @minor = @build = @revision = 0
			if version
				parts = version.split('.')
				@major = parts[0] if parts[0]
				@minor = parts[1] if parts[1]
				@build = parts[2] if parts[2]
				@revision = parts[3] if parts[3]	
			end
		end
		def to_s
			"#{@major}.#{@minor}.#{@build}.#{@revision}"
		end
	end
	
	class AssemblyInfoTask < TaskBase 
		attr_accessor :version
		
		def init
			@version = Version.new
			@properties = Hash.new
		end
		
		def filename=(value)
			@filename = value
		end
		def filename
			@filename || 'src/CommonAssemblyInfo.cs'
		end
		
		# hack to collect Copyright, Company etc.
		def method_missing(m, *args)
			super if args.empty?
			key = m.to_s.gsub('=', '').capitalize
			value = args[0]
			@properties[key] = value
		end
		
		def execute
			flash "marking this build as version #{version.to_s}"
			@properties['Version'] = @properties['InformationalVersion'] = @properties['FileVersion'] = version.to_s; 
			template = %q{using System.Reflection;
using System.Runtime.InteropServices;
[assembly: ComVisible(false)]
<% @properties.each {|k, v| %>
[assembly: Assembly<%=k%>Attribute("<%=v%>")]
<% } %>}
		  
		  erb = ERB.new(template, 0, "%<>")
		  
		  File.open(filename, 'w') do |file|
			  file.puts erb.result(binding()) 
		  end
		end
	end
	
	class MsBuildTask < ToolTask
		remove_tool_attr # hide command_line attribute, and make tool_name readonly
		private :tool_path= # also make tool_path readonly
		attr_accessor :verbosity_level, :project, :toolsversion, :targets, :properties 
		def init
			super
			@tool_name = 'msbuild.exe'
			@tool_path = File.join('c:', 'windows', 'Microsoft.NET', 'Framework', 'v3.5')
			@properties = Hash.new
			@toolsversion = '3.5'
			@targets = []
			@verbosity_level = 'minimal'
		end
		def tool_args
			raise "No project has been passed to MsBuild" if nil_or_empty?(@project)
			flash "compiling #{File.basename(@project)}"
			args  = "#{project} /maxcpucount"
			args += " /toolsversion:#{@toolsversion}"
			args += " /verbosity:#{@verbosity_level}"
			if @targets.nil? || @targets.empty?
				args += " /target:Build" 
			else
				args += " /target:#{@targets.join(',')}"
			end
			args += " /property:BuildInParallel=false"
			if @properties.nil?
				args += " /property:Configuration=debug"
			else
				@properties.each { |k,v| args += " /property:#{k}=#{v}"  }		
			end
			args
		end
		def after_execute 
			fail ("MsBuild returned exit code #{exit_code}. Check the logged output to find the error(s).") unless exit_code == 0	
		end 
	end
	
	class FxCopTask < ToolTask
		remove_tool_attr # hide command_line attribute, and make tool_name readonly
		attr_accessor :assemblies
		attr_accessor :assembly_search_path
		attr_accessor :results_file
		attr_accessor :rich_console_output
		def init
			super
			@tool_name = 'FxCopCmd.exe'
			@assemblies = []
			@rich_console_output = false
			@error_messages = {
				0x0 => 'No errors',
				0x1 => 'Analysis error',	
				0x2 => 'Rule exceptions',	
				0x4 => 'Project load error',	
				0x8 => 'Assembly load error',	
				0x10 => 'Rule library load error',	
				0x20 => 'Import report load error',	
				0x40 => 'Output error',	
				0x80 => 'Command line switch error',	
				0x100 => 'Initialization error',	
				0x200 => 'Assembly references error',	
				0x400 => 'BuildBreakingMessage',	
				0x1000000 => 'Unknown error'	
			}
		end
		def tool_args
			raise "No assemblies have been passed to FxCop" if nil_or_empty?(@assemblies)
			raise "No results_file have been passed to FxCop" if nil_or_empty?(@results_file)
			raise "No assembly_search_path have been passed to FxCop" if nil_or_empty? @assembly_search_path
			flash "performing static analysis on:"
			args = ""
			@assemblies.each do |assembly| 
				puts "#{File.basename(assembly)}"
				args += " " unless nil_or_empty?(args)
				args += "/file:#{assembly}"
			end
			args += " /rule:#{File.join(@tool_path, 'Rules')}"
			args += " /out:#{@results_file}"
			args += " /dictionary:#{File.join(@tool_path, 'CustomDictionary.xml')}"
			args += " /outxsl:none"
			args += " /consolexsl:#{File.join(@tool_path, 'Xml', 'FxCopRichConsoleOutput.xsl')}" if @rich_console_output
			args += " /directory:#{@assembly_search_path}" unless nil_or_empty? @assembly_search_path
			args += " /timeout:120 /ignoregeneratedcode /summary /forceoutput"
			args
		end
		def after_execute
			msg = @error_messages.fetch(exit_code, 'Unknown exit code')
			puts "FxCop exited with exit code #{exit_code}: #{msg}"
			# Parse results file using XPath
			result = FxCopReport.new(@results_file)
			puts "FxCop encountered #{result.total} rule violation(s):"
			puts "  Critical errors: #{result.critical_errors}"
			puts "  Errors: #{result.errors}"
			puts "  Critical warnings: #{result.critical_warnings}"
			puts "  Warnings: #{result.warnings}"
			# Generate html reports
			stylesheet_folder = File.join(tool_path, 'Xml')
			results_folder = File.dirname(results_file)
			report_stylesheet = File.join(stylesheet_folder, 'FxCopReport.xsl')
			graph_stylesheet = File.join(stylesheet_folder, 'FxCopGraph.xsl') 
			XslTransform.save(results_file.ext('html'), :xml => results_file, :xslt => report_stylesheet)
			graph_html = File.join(results_folder, 'FxCopGraph.html')
			XslTransform.save(graph_html, :xml => results_file, :xslt => graph_stylesheet)
			dest = File.join(results_folder, 'images')
			Dir.mkdir(dest) unless File.exists?(dest)
			cp_wo_svn File.join(stylesheet_folder, 'images'), dest
		end
	end
	
	class FxCopReport
		include TaskUtils
		
		def initialize(file)
			@doc = XPathDoc.new(file)	
		end
		def critical_errors
			count('CriticalError')
		end
		def errors
			count('Error')
		end
		def critical_warnings
			count('CriticalWarning')
		end
		def warnings
			count('Warning')
		end
		def count(level)
			@cache ||= Hash.new
			return @cache[level] if @cache.has_key? level 
			value = @doc.find("string(count(//Issue[@Level='#{level}']))")
			c = value.to_i
			@cache[level] = c
			c
		end
		def total
			critical_errors + errors + critical_warnings + warnings	
		end	
	end
	
	class SimianTask < ToolTask
		remove_tool_attr # hide command_line attribute, and make tool_name readonly
		attr_accessor :threshold # the minimum number of duplicate lines that violates our policy
		attr_accessor :fail_on_duplication
		attr_accessor :results_file
		attr_accessor :stylesheet
		def init
			super
			@tool_name = 'simian-2.2.24.exe'
			@threshold = 3
			@fail_on_duplication = false
		end
		def tool_args
			flash "performing similarity analysis"
			args = "-includes=src/**/*.cs"
			args += " -threshold=#{@threshold}"
			if @fail_on_duplication
				args += " -failOnDuplication+"
			else
				args += " -failOnDuplication-"
			end
			args += " -formatter=xml:#{@results_file}"
			args	
		end
		def after_execute
			XslTransform.save(results_file.ext('html'), :xml => results_file, :xslt => stylesheet) unless nil_or_empty? stylesheet
		end
	end
	
	# with code coverage
	class XUnitTask < ToolTask
		remove_tool_attr # hide command_line attribute, and make tool_name readonly
		#todo: make two helper classes to carry the params
		attr_accessor :calculate_coverage
		attr_accessor :results_folder
		attr_accessor :test_assembly, :test_results_filename, :nunit_test_results_filename, :test_stylesheet, :nunit_test_stylesheet
		attr_accessor :coverage_assemblies, :coverage_results_filename, :coverage_exclude_attrs, :coverage_log_filename, :coverage_verbose_logging
		def init
			super
			@coverage_assemblies = []
			@coverage_exclude_attrs = []
			@coverage_verbose_logging = false
		end
		def test_results_file
			File.join(@results_folder, @test_results_filename)
		end
		def nunit_test_results_file
			File.join(@results_folder, @nunit_test_results_filename) unless nil_or_empty? @nunit_test_results_filename
		end
		def coverage_results_file
			File.join(@results_folder, @coverage_results_filename)
		end
		def command
			fail "No tool_path passed to xunit task" if nil_or_empty? @tool_path
			fail "No test_assembly was passed to xunit task" if nil_or_empty? @test_assembly
			fail "No results_folder passed to xunit task" if nil_or_empty? @results_folder
			fail "No test_results_filename passed to xunit task" if nil_or_empty? @test_results_filename
			fail "No coverage_results_filename passed to xunit task" if @calculate_coverage and nil_or_empty? @coverage_results_filename
			
			# if Array or FileList object we pick the first item, otherwise we just pick whatever object 
			test_asm = first(@test_assembly)
			
			flash "running #{File.basename(test_asm).ext} and calculating code coverage"
			
			coverage_log_file = File.join(@results_folder, @coverage_log_filename) unless nil_or_empty? @coverage_log_filename
			
			xunit_path = File.join(@tool_path, 'xunit')
			if ! nil_or_empty? nunit_test_results_file
				xunit_cmd = ToolCommand.new(xunit_path, 'xunit.console.exe', test_asm, ' ', :noshadow => nil, :xml => test_results_file, :nunit => nunit_test_results_file).to_s
			else
				xunit_cmd = ToolCommand.new(xunit_path, 'xunit.console.exe', test_asm, ' ', :noshadow => nil, :xml => test_results_file).to_s
			end

			return xunit_cmd unless @calculate_coverage

			###############################################################################
			# Note: NCover is expecting assembly names (not filenames) to be passed with 
			# the //a switch. If filenames are used then an empty coverage.xml file will  
			# be generated. For more information see:
			#     http://www.kiwidude.com/blog/2007/04/ncover-problems-fixes-part-2.html
			#
			# FileList objects are lazy, and the pathmap method will collect the filenames of 
			# the assemblies without the extension. Therefore the pathmap method will resolve 
			# the FileList objects internal patterns to a list of files on the disk, and therefore
			# it is important to do this when the task executes. 
			###############################################################################
			coverage_assembly_names = @coverage_assemblies.pathmap('%n')

			cmd = File.join(@tool_path, 'ncover', 'NCover.Console.exe')
			cmd += " #{xunit_cmd}"
			cmd += " //a #{coverage_assembly_names.to_a.join("\;")}" unless nil_or_empty? coverage_assembly_names
			cmd += " //ea #{@coverage_exclude_attrs.join("\;")}" unless nil_or_empty? @coverage_exclude_attrs
			cmd += " //w #{@working_folder}" unless nil_or_empty? @working_folder
			cmd += " //l #{coverage_log_file}" unless nil_or_empty? coverage_log_file
			cmd += " //v" if @coverage_verbose_logging
			cmd += " //x #{coverage_results_file}"
			cmd += " //reg" # see http://www.kiwidude.com/blog/2007/04/ncover-problems-fixes-part-1.html	
			cmd
		end
		def after_execute
			XslTransform.save(coverage_results_file.ext('html'), :xml => coverage_results_file, :xslt => File.join(tool_path, 'ncover', 'Coverage.xsl')) if @calculate_coverage
			
			XslTransform.save(test_results_file.ext('html'), :xml => test_results_file, :xslt => test_stylesheet) unless nil_or_empty? test_stylesheet
			
			XslTransform.save(nunit_test_results_file.ext('html'), :xml => nunit_test_results_file, :xslt => nunit_test_stylesheet) unless nil_or_empty? nunit_test_stylesheet
			
			fail ("There are test failures. The testrunner exited with code #{exit_code}. Check the logged output to find the failed test(s).") unless exit_code == 0
		end
	end
	
	class ToolCommand
		attr_accessor :tool_path, :tool_name, :input_file, :sep_token
		# options is a hash (named params) that define the switches
		def initialize(tool_path, tool_name, input_file, sep_token, options)
			@tool_path, @tool_name, @input_file = tool_path, tool_name, input_file
			@sep_token = sep_token
			@options = (options || Hash.new)
		end
		def to_s
			cmd = File.join(tool_path, tool_name)
			cmd += " #{input_file}" if input_file
			@options.each do |k, v| 
				cmd += " /#{k}"
				cmd += "#{@sep_token}#{v}" if v   
			end
			cmd
		end	
	end
	
	# Report and Merge tool 
	class NCoverExplorerTask < ToolTask
		attr_accessor :coverage_files # the ncover coverage file to create summary report on
		attr_accessor :merged_results_filename # used to merge two or more coverage_files
		attr_accessor :results_folder, :xml_results_filename, :html_results_filename, :report_type  # report type
		attr_accessor :project # the name of the project (appears in the report)
		attr_accessor :sort, :filter # see doc 
		attr_accessor :min_coverage
		attr_accessor :flash_message
		def init
			super
			@tool_name = 'NCoverExplorer.Console.exe'
			@report_type = 'ModuleClassSummary'
			@coverage_files = []
			@error_messages = { 
				0 => 'OK',
				2 => 'Exception',
				3 => 'Failed minimum coverage threshold check'
			}
		end
		def tool_args
			fail "No coverage_files passed to NCoverExplorer" if nil_or_empty? @coverage_files
			fail "No results_folder passed to NCoverExplorer" if nil_or_empty? @results_folder
			flash @flash_message unless nil_or_empty? @flash_message
			xml_results_file = File.join(@results_folder, @xml_results_filename) unless nil_or_empty? @xml_results_filename 
			html_results_file = File.join(@results_folder, @html_results_filename) unless nil_or_empty? @html_results_filename
			merged_results_file = File.join(@results_folder, @merged_results_filename) unless nil_or_empty? @merged_results_filename
			args = "#{@coverage_files.join(' ')}"
			args += " /project:\"#{@project}\"" unless nil_or_empty? @project
			# report tool args
			args += " /xml:#{xml_results_file}" unless nil_or_empty? html_results_file
			args += " /html:#{html_results_file}" unless nil_or_empty? html_results_file
			args += " /report:#{@report_type}" unless (nil_or_empty?(html_results_file) && nil_or_empty?(xml_results_file))
			args += " /sort:#{@sort}" unless nil_or_empty? @sort
			args += " /filter:#{@filter}" unless nil_or_empty? @filter
			args += " /minCoverage:#{@minCoverage}" unless nil_or_empty? @min_coverage
			# merge tool args
			args += " /save:#{merged_results_file}" unless nil_or_empty? merged_results_file
			args
		end
		def after_execute
			puts "#{tool_name} exited with code #{exit_code}: #{@error_messages.fetch(exit_code, 'Unknown exit code')}"
			cp File.join(tool_path, 'CoverageReport.xsl'), results_folder
		end
	end
	
	class NAntTask < ToolTask
		def init
			super
			@tool_name = 'nant.exe'
		end
	end
	
	class RakeTask < ToolTask
		def init
			super
			@tool_name = "rake"
		end
	end
	
	class SvnInfoTask < ToolTask 		
		def init
			super
			@tool_name = 'svn.exe'
		end
		
		def tool_args
			"info"
		end
	end
	
	class SvnStatusTask < ToolTask 		
		def init
			super
			@tool_name = 'svn.exe'
		end
		
		def tool_args
			"status"
		end
	end
	
	class SvnUpdateTask < ToolTask 		
		attr_accessor :revision
		
		def init
			super
			@tool_name = 'svn.exe'
		end
		
		def tool_args
			"update -r#{revision}"
		end
	end
	
	class SvnCheckoutTask < ToolTask 		
		attr_accessor :revision
		attr_accessor :url
		attr_accessor :folder

		def init
			super
			@tool_name = 'svn.exe'
		end
		
		def tool_args
			"checkout #{url} -r#{revision} #{folder}"
		end
	end
	
	class DatabaseManagerRunner
		attr_accessor :action, :script_dir, :server, :database
		def initialize(tool_path, script_dir, server, database)
			@tool_path = tool_path
			@script_dir, @server, @database = script_dir, server, database
		end
		def command
			tool_cmd = ToolCommand.new(@tool_path, 'Tarantino.DatabaseManager.Console.exe', nil, ':',
				:Action => @action,
				:ScriptDirectory => @script_dir,
				:Server => @server,
				:Database => @database)
			tool_cmd.to_s
		end
		def execute(action = nil)
			@action = action if action
			cmd = command 
			puts cmd
			cout = `#{cmd}`
			@exit_code = $?.exitstatus
			unless @exit_code == 0
				fail "Command failed with status (#{@exit_code}): [#{cmd}]"
			end
			cout =~ /NewDatabaseVersion: (\d+)/
			@new_database_version = $1  
		end
		def exit_code
			@exit_code
		end
		def new_database_version
			@new_database_version
		end
	end
	
end