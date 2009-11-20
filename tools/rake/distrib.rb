#!/usr/bin/env ruby

require 'tools/rake/tasks'

module Rake
	
	class DistribTask < TaskBase
		attr_accessor :source_folder
		attr_accessor :source_project
		
		def distrib_to(project, dest_folder)
			@projects ||= Hash.new
			@projects[dest_folder] = project 
			@dest_folders ||= []
			d = DistribDestFolder.new(dest_folder)
			yield d if block_given?
			@dest_folders << d
		end
		
		def execute
			@dest_folders.each do |dest_folder|  
				project = @projects[dest_folder.to_s]
				print "\nDistributing #{source_project.nil? ? '' : source_project + ' '}build artifacts to #{project || 'unknown project'}:\n\n"
				dest_folder.filenames do |filename|
					cp "#{normalize(File.join(@source_folder, filename))}", "#{normalize(File.join(dest_folder.to_s, filename))}"
				end
			end		
		end
	end

	class DistribDestFolder
		def initialize(dest_folder)
			@dest_folder = dest_folder
		end
		def add(basefilename)
			@files ||= []
			d = DistribFilename.new(basefilename)
			@files << d
			d		
		end
		def to_s
			@dest_folder	
		end
		def filenames
			@files.each do |f|
				f.filenames do |filename|
					yield filename
				end
			end
		end
	end

	class DistribFilename
		def initialize(basefilename)
			@basefilename = basefilename
			@extensions = []
		end
		def with_ext(*extensions)	
			extensions.each { |ext| @extensions << ext } if extensions
			self
		end
		def filenames
			@extensions.each { |ext| yield "#{@basefilename}.#{ext}"  }
		end
	end

end