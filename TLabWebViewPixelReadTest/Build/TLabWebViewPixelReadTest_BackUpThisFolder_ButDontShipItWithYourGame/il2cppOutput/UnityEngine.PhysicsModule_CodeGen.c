#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





// 0x00000001 UnityEngine.Collider UnityEngine.RaycastHit::get_collider()
extern void RaycastHit_get_collider_m84B160439BBEAB6D9E94B799F720E25C9E2D444D (void);
// 0x00000002 UnityEngine.Vector3 UnityEngine.RaycastHit::get_point()
extern void RaycastHit_get_point_m02B764612562AFE0F998CC7CFB2EEDE41BA47F39 (void);
// 0x00000003 UnityEngine.Vector3 UnityEngine.RaycastHit::get_normal()
extern void RaycastHit_get_normal_mD8741B70D2039C5CAFC4368D4CE59D89562040B5 (void);
// 0x00000004 System.Single UnityEngine.RaycastHit::get_distance()
extern void RaycastHit_get_distance_m035194B0E9BB6229259CFC43B095A9C8E5011C78 (void);
static Il2CppMethodPointer s_methodPointers[4] = 
{
	RaycastHit_get_collider_m84B160439BBEAB6D9E94B799F720E25C9E2D444D,
	RaycastHit_get_point_m02B764612562AFE0F998CC7CFB2EEDE41BA47F39,
	RaycastHit_get_normal_mD8741B70D2039C5CAFC4368D4CE59D89562040B5,
	RaycastHit_get_distance_m035194B0E9BB6229259CFC43B095A9C8E5011C78,
};
extern void RaycastHit_get_collider_m84B160439BBEAB6D9E94B799F720E25C9E2D444D_AdjustorThunk (void);
extern void RaycastHit_get_point_m02B764612562AFE0F998CC7CFB2EEDE41BA47F39_AdjustorThunk (void);
extern void RaycastHit_get_normal_mD8741B70D2039C5CAFC4368D4CE59D89562040B5_AdjustorThunk (void);
extern void RaycastHit_get_distance_m035194B0E9BB6229259CFC43B095A9C8E5011C78_AdjustorThunk (void);
static Il2CppTokenAdjustorThunkPair s_adjustorThunks[4] = 
{
	{ 0x06000001, RaycastHit_get_collider_m84B160439BBEAB6D9E94B799F720E25C9E2D444D_AdjustorThunk },
	{ 0x06000002, RaycastHit_get_point_m02B764612562AFE0F998CC7CFB2EEDE41BA47F39_AdjustorThunk },
	{ 0x06000003, RaycastHit_get_normal_mD8741B70D2039C5CAFC4368D4CE59D89562040B5_AdjustorThunk },
	{ 0x06000004, RaycastHit_get_distance_m035194B0E9BB6229259CFC43B095A9C8E5011C78_AdjustorThunk },
};
static const int32_t s_InvokerIndices[4] = 
{
	3112,
	3187,
	3187,
	3147,
};
IL2CPP_EXTERN_C const Il2CppCodeGenModule g_UnityEngine_PhysicsModule_CodeGenModule;
const Il2CppCodeGenModule g_UnityEngine_PhysicsModule_CodeGenModule = 
{
	"UnityEngine.PhysicsModule.dll",
	4,
	s_methodPointers,
	4,
	s_adjustorThunks,
	s_InvokerIndices,
	0,
	NULL,
	0,
	NULL,
	0,
	NULL,
	NULL,
	NULL, // module initializer,
	NULL,
	NULL,
	NULL,
};
