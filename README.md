# FileDownloadIntegrityChecker
This (.NET 6) program utilizes asynchronous methods to download a file and calculate/validate its checksums. 

For the file, a linux CD ISO is used to simulate any file of non-negligible size.

A more storage-dependent example can be achieved by changing the "CD" in the URL on line 84 of MainVM.cs to "DVD" instead,
this will make it easier to see the download completion reflected in the progress bars if your internet connection is fast.
