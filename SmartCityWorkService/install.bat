sc create SmartCityWorkService binPath= %~dp0SmartCityWorkService.exe
sc failure SmartCityWorkService actions= restart/60000/restart/60000/""/60000 reset= 86400
sc start SmartCityWorkService
sc config SmartCityWorkService start= delayed-auto