import Clear

#class PythonDrivers:
#right now you must make an instance of each driver type so that the .NET type can be queried
pythonDrivers = [
    Clear.Clear()
    ]

def loadDrivers(driverList):
    for driver in pythonDrivers:
        driverList.Add(driver.GetType())