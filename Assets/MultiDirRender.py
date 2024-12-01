import bpy
import os
import math
import numpy as np
import re
import mathutils


def render8dirs_selected_obj(path, num_dirs):
    # ensure path exists
    render_folder = os.path.abspath(path)
    if not os.path.exists(render_folder):
        os.makedirs(render_folder)

    selected_list = bpy.context.selected_objects
    
    # only select first selected obj
    bpy.ops.object.select_all(action='TOGGLE')
    bpy.context.scene.objects[selected_list[0].name].select_set(True)

    s=bpy.context.scene

    files = []
    prev_rotation = bpy.context.active_object.rotation_euler[2]
    file_idx = 0
    angle = 360.0
    while angle > 0:
        # rotate obj for this angle
        bpy.context.active_object.rotation_euler[2] = math.radians(angle)
    
        s.render.filepath = os.path.join(render_folder, str(file_idx) + ".png")
        
        bpy.ops.render.render(
                              False,
                              animation=False,
                              write_still=True
                             )
        files.append(s.render.filepath)
        
        file_idx += 1
        angle -= 360/num_dirs

    bpy.context.active_object.rotation_euler[2] = prev_rotation
    return files

def remove_renders_from_dir(path):
    path = os.path.abspath(path)
    render_files = []
    for file in os.listdir(path):
        if(re.search("^[0-9]+.(png)", file)):
            os.remove(os.path.join(path, file))

def merge_images(image_files, out_path):
    out_path = os.path.abspath(out_path)
    final_image_pixels = []

    final_image_width = 0
    final_image_height = 0

    for file in reversed(image_files):
        img = bpy.data.images.load(file, check_existing=True)
        loaded_pixels = img.pixels
        final_image_width = img.size[1]
        final_image_height += img.size[0]
        final_image_pixels.extend(np.array(loaded_pixels))
        bpy.data.images.remove(img)
    
    # apply sRGB colorspace
    for i in range(0, len(final_image_pixels), 4):
            color = mathutils.Color(final_image_pixels[i:i+3]).from_scene_linear_to_srgb()
            final_image_pixels[i] = color.r
            final_image_pixels[i+1] = color.g
            final_image_pixels[i+2] = color.b

    # export spritesheet
    out_img = bpy.data.images.new("SPRITESHEET", alpha=True, width=final_image_width, height=final_image_height)
    out_img.file_format = "PNG"
    out_img.alpha_mode = 'STRAIGHT'
    out_img.pixels = final_image_pixels
    out_img.filepath_raw = os.path.join(out_path, "SPRITESHEET.png")
    out_img.save_render(out_img.filepath_raw)
    bpy.data.images.remove(out_img)
    
    
path = "multi-direction-renders"
num_dirs = 16
remove_renders_from_dir(path)
files = render8dirs_selected_obj(path, num_dirs)
merge_images(files, path)
