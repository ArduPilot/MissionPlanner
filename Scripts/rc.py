

print 'Start Script'
for chan in range(1,9):
    Script.SendRC(chan,1500,False)

Script.SendRC(3,1500,True)
Script.Sleep(1000)
Script.SendRC(3,1100,True)