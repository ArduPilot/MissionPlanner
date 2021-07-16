import bpy
import os

basedir = os.path.dirname(bpy.data.filepath) + '\\'

if not basedir:
    raise Exception("file is not saved")

headerOfPLY = "name x y z r g b"

scene = bpy.context.scene

print (scene)

comma2=''
steps=[]
json = open(basedir + 'test.txt', 'w');
json.write('{     "Layouts": [ ')

for frame in range(scene.frame_start, scene.frame_end, 24):
    scene.frame_set(frame)
    scene.update()
    the_file = "pointCloud_f" + str('{:03d}'.format(scene.frame_current)) + ".ply"
    print("loading "+basedir + the_file+'\n')
    with open(basedir + the_file, 'w') as wFile:
        json.write(comma2 + '{ "Offset": {\n')
        comma = ''
        for obj in bpy.data.objects:
            numbs = [int(s) for s in obj.name.replace("_"," ").split(" ") if s.isdigit()]
            if len(numbs) == 0:
                continue
            vertex = obj.matrix_world.to_translation()
            #color = [ 0, 0, 0]
            if obj.active_material != None:
                color = obj.active_material.diffuse_color
            wFile.write('%s %f %f %f %d %d %d\n' %(obj.name, vertex.x,vertex.y,vertex.z, color[0], color[1], color[2]))
            json.write('\t' + comma + ' "%s": { "x": %f, "y": %f, "z": %f }\n' %(numbs[0], vertex.x,vertex.y,vertex.z))
            comma = ','
        steps.append(str('{:03d}'.format(scene.frame_current)))
        json.write('}, "Id": "'+str('{:03d}'.format(scene.frame_current))+'" }')
        comma2 = ','
    print("writing completed!")
    wFile.close()
json.write('],\n"Steps": [')
comma = ''
for step in steps:
    json.write(comma + '"%s"\n'%(step))
    comma = ','
json.write(']}')
json.close()
