//
//  UnityInterface.cpp
//  SturGProcess
//
//  Created by Patrick Metcalfe on 3/20/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#include "UnityInterface.hpp"

//void clearUnneededResources();
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
clearUnneededResources() {
    currentInvocation.vertices.clear();
    currentInvocation.references.clear();
    currentInvocation.faces.clear();
    currentInvocation.tileCenter.clear();
}
#define BUILD_NUMBER "8"
//long processSturGFileData(System.IntPtr /* byte[] */ sturGDataPoint, long length);
// NOTE: CALLED FIRST
extern "C" long UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
processSturGFileData(char* sturGData, int length) {
    std::cout << "Patrick Build Version: " << BUILD_NUMBER << std::endl;
    clearUnneededResources();
    SturGModel model = currentInvocation.processor.processFromPointer(sturGData, (size_t)length);
    
    int buildingCount = (int)model.buildings.size();
    int vertexCount = (int)model.vertexCount;
    int facesCount = (int)model.facesCount;
    
    currentInvocation.vertices.reserve(model.vertexCount * 3);
    currentInvocation.faces.reserve(model.facesCount * 3);
    
    int vertexTally = 0;
    int facesTally = 0;
    
    std::vector<BuildingReference> buildingReferences;
    std::transform(model.buildings.begin(), model.buildings.end(), std::back_inserter(buildingReferences), [&vertexTally, &facesTally](SturGBuilding building) {
        int vertexStart = vertexTally;
        int faceStart = facesTally;
        std::cout << "Vertex/Faces Start: " << vertexStart << ", " << faceStart << std::endl;
        std::for_each(building.vertices.begin(), building.vertices.end(), [&vertexTally](Point3<float> vertex){
            vertexTally += 3;
             std::cout << "Vertex " << vertex[0] << ", " << vertex[1] << ", " << vertex[2] << std::endl;
            currentInvocation.vertices.push_back(vertex[0]);
            currentInvocation.vertices.push_back(vertex[1]);
            currentInvocation.vertices.push_back(vertex[2]);
        });
        
        std::for_each(building.faces.begin(), building.faces.end(), [&facesTally](Point3<uint16_t> face){
            facesTally += 3;
            std::cout << "Face " << face[0] << ", " << face[1] << ", " << face[2] << std::endl;
            currentInvocation.faces.push_back(face[0]);
            currentInvocation.faces.push_back(face[1]);
            currentInvocation.faces.push_back(face[2]);
        });
        std::cout << "Vertex/Faces End: " << vertexTally << ", " << facesTally << std::endl;
        return BuildingReference((int)building.descriptor.id, vertexStart, vertexTally, faceStart, facesTally);
    });
    
    currentInvocation.references = buildingReferences;
    currentInvocation.tileCenter = std::vector<float>(model.metadata.tileCenter.begin(), model.metadata.tileCenter.end());
    
    // NOTE: vertex/faces.size() is the correct value here
    
    std::cout << "Building Marshalling (debug): (Building Count, Vertex Count, Faces Count): (" << buildingCount << ", " << vertexCount << ", " << facesCount << ")" << std::endl;
    std::cout << "Building Marshalling (debug): From long way (Building Count, Vertex Tally, Faces Tally): (" << buildingCount << ", " << vertexTally << ", " << facesTally << ")" << std::endl;
    std::cout << "Building Marshalling (debug): From vectors (Building Count, Vertex Count, Faces Count): (" << currentInvocation.references.size() << ", " << currentInvocation.vertices.size() << ", " << currentInvocation.faces.size() << ")" << std::endl;
    std::cout << "Building Marshalling (debug): Cantor Encoded Version: (" << cantorEncode3D(buildingCount, vertexCount, facesCount) << ")" << std::endl;
    return cantorEncode3D(buildingCount, vertexTally, facesTally);
}

//NOTE CALLED FOURTH AND RETURNS 0
//int getVertexLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getVertexLength() {
    std::cout << "Handing over Vertex Length of " << currentInvocation.references.size() << std::endl;
    return (int)currentInvocation.vertices.size();
}

//int getFacesLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getFacesLength() {
    std::cout << "Handing over Faces Length of " << currentInvocation.references.size() << std::endl;
    return (int)currentInvocation.faces.size();
}

// NOTE: CALLED SECOND AND RETURNS PROPER VALUE
//int getBuildingsLength();
extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
getBuildingsLength() {
    std::cout << "Handing over Building Length of " << currentInvocation.references.size() << std::endl;
    return (int)currentInvocation.references.size();
}

//void fillTileCenter(out System.IntPtr /* float[] */ tileCenterPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillTileCenter(float** tileCenterPointer, int length) {
    std::cout << "Fill Tile Center Called with length " << length << " while length should be " << currentInvocation.tileCenter.size() << std::endl;
    *tileCenterPointer = currentInvocation.tileCenter.data();
}

//NOTE: CALLED THIRD AND RETURNS CORRECT STUFF
//void fillBuildingReferences(out System.IntPtr /* BuildingReference[] */ buildingReferencePointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillBuildingReferences(BuildingReference** buildingReferences, int length) {
    std::cout << "Fill Building References Called with length " << length << " while length should be " << currentInvocation.references.size() << std::endl;
    *buildingReferences = currentInvocation.references.data();
}

// CALLED FIFTH AND THE VALUES IN THE VECTOR ARE CORRECT BUT IT STILL SAYS SIZE IS 0. SAME WITH FACES
//void fillVertexData(out System.IntPtr /* float[] */ vertexDataPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillVertexData(float** vertexDataPointer, int length) {
    std::cout << "Fill Vertex Data Called with length " << length << " while length should be " << currentInvocation.vertices.size() << std::endl;
    for (size_t i = 0; i <= 6; i++) {
        std::cout << "Vertex Given was " << currentInvocation.vertices[i] << std::endl;
    }
    *vertexDataPointer = currentInvocation.vertices.data();
}

//void fillFaceData(out System.IntPtr /* int[] */ faceDataPointer, long length);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
fillFaceData(int** faceDataPointer, int length) {
    std::cout << "Fill Face Data Called with length " << length << " while length should be " << currentInvocation.faces.size() << std::endl;
    for (size_t i = 0; i <= 6; i++) {
        std::cout << "Face Given was " << currentInvocation.faces[i] << std::endl;
    }
    *faceDataPointer = currentInvocation.faces.data();
}
