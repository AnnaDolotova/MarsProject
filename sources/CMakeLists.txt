cmake_minimum_required(VERSION 2.8) 
set (PROJECT WindowsFormsApplication4)                                    
project(${PROJECT})

set (SOURCE Program.cs Form1.cs Form1_Designer.cs LoaderGDI.cs Mesh.cs)
set(LIBRARIES OpenTK.dll System.Drawing.dll)
add_executable(${PROJECT} ${SOURCE})
target_link_libraries (${PROJECT} ${LIBRARIES})
#SET_TARGET_PROPERTIES(${PROJECT} PROPERTIES LINKER_LANGUAGE CS)