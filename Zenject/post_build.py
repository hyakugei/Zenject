
import sys
import shutil
import os

_args = None

class Args:
    def __init__ (self):
        self.targetName = None
        self.targetDir = None
        self.destinationDir = None

def ParseArgs():
    args = Args()

    if len(sys.argv) != 4:
        print "Invalid # of arguments"
        exit(-1)

    args.targetName = sys.argv[1]
    args.targetDir = sys.argv[2]
    args.destinationDir = sys.argv[3]

    return args

def CopyFile(sourcePath, destinationPath):

    try:
        shutil.copy2(sourcePath, destinationPath)
        print "Successfully copied file '"+ sourcePath + "' to '" + destinationPath + "'"
    except Exception, e:
        # Use a visual studio friendly error so it pops up as an error
        print "error MSB3021: Unable to copy file \"" + sourcePath + "\" to \"" + destinationPath + "\"."

def Run():
    scriptDir = os.path.dirname(os.path.realpath(__file__))

    file1 = _args.targetName + ".dll"
    file2 = _args.targetName + ".pdb"

    CopyFile(_args.targetDir + file1, _args.destinationDir + file1)
    CopyFile(_args.targetDir + file2, _args.destinationDir + file2)

_args = ParseArgs()
Run()
