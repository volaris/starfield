from subprocess import call
from subprocess import Popen

#todo: this needs to be a subprocess.popen and it needs to clean up the subprocess when canceled
call(["python", "-m", "SimpleHTTPServer"])
call(["python", "grab_images.py"])