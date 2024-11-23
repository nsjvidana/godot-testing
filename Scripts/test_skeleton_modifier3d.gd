@tool
extends SkeletonModifier3D
class_name TestSkeletonModifier3D

var skeleton;
var spine_idx;

func _ready() -> void:
    skeleton = get_skeleton();
    var bone_names = skeleton.get_concatenated_bone_names().split(",");
    spine_idx = -1;
    var idx = 0;
    for bone_name in bone_names:
        if bone_name.ends_with("Spine"):
            spine_idx = idx;
            break;
        idx += 1;
    modification_processed.connect(
        func(): 
            var ae = skeleton.get_bone_global_pose(spine_idx).origin;
            ae.x = 0;
    );
