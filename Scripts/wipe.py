# cs.???? = currentstate, any variable on the status tab in the planner can be used.
# Script = options are 
# Script.Sleep(ms)
# Script.ChangeParam(name,value)
# Script.GetParam(name)
# Script.ChangeMode(mode) - same as displayed in mode setup screen 'AUTO'
# Script.WaitFor(string,timeout)
# Script.SendRC(channel,pwm,sendnow)
# 

print 'Start Script'

mrev = Script.GetParam("SYSID_SW_MREV")
fmtv = Script.GetParam("FORMAT_VERSION")
if mrev > 0:
    print Script.ChangeParam("SYSID_SW_MREV",0)
if fmtv > 0:
    print Script.ChangeParam("FORMAT_VERSION",0)

Script.Sleep(1)

MAV.doReboot()

print 'Reset complete'