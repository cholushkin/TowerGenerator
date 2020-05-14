set blender_path_clean=%blender_path:"=%
set unity_import_path_clean=%unity_import_path:"=%

"%blender_path_clean%" %1 -b -P export_fbx.py -- %unity_import_path_clean%\%2