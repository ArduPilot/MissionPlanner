#!/usr/bin/ipy
import clr

import System
clr.AddReference("System.Windows.Forms")
#clr.AddReference("System.Drawing")
clr.AddReference("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
clr.AddReference("System")

clr.ClearProfilerData() 

from System.Windows.Forms import Application, Form, Label
from System.Drawing import Size,  Point, Font, Size

print "MissionPlanner.Drawing" in [assembly.GetName().Name for assembly in clr.References]
print "System.Drawing" in [assembly.GetName().Name for assembly in clr.References]

for index, item in enumerate(clr.References):
    if item.GetName().Name == "System.Drawing":
        drawing = item
font = drawing.GetType("System.Drawing.Font")
print dir(font)
font = System.Activator.CreateInstance(font, "Arial", 12)
print "dir ", dir(font)
####################################
#    GUI Iform CODE STARTS HERE    #
####################################

class IForm(Form):
    def __init__(self):
            
        self.my_label = Label() 
        self.my_label.Text=  "Fonts work"   
        self.my_label.Location = Point(10,35)
        self.my_label.Height = 30
        self.my_label.Width = self.Width - 150
        try:
            self.my_label.Font = System.Drawing.Font("Arial", 12)
        except: 
            self.my_label.Text = "font not found"
        self.Controls.Add(self.my_label)
        
        

        self.my_label.Font = font #Font("Arial", 20)
    
        
        
##############################
#   end    GUI stuff         #
##############################            
       
   
Application.Run(IForm())  #run the Iform...
