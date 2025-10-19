## 放置引擎-游戏资源
extends Resource
class_name IdleFramework_GameResource

## 空间注册表，在该列表中添加项即可注册空间
@export var space_registry: Dictionary[StringName, IdleFramework_Space]

## 容器注册表，在该列表中添加项即可注册容器
## 现在容器在空间中注册
#@export var container_registry: Dictionary[StringName, IdleFramework_Container]

## 物品注册表，在该列表中添加项即可注册物品
@export var item_registry: Dictionary[StringName, IdleFramework_ItemRegistryObject]

## 配方注册表，在该列表中添加项即可注册配方
@export var recipe_registry: Dictionary[StringName, IdleFramework_Recipe]

## 工厂注册表，在该列表中添加项即可注册工厂
## 现在工厂在空间中注册
#@export var factory_registry: Dictionary[StringName, IdleFramework_Factory]

## 附加数据，可用于存储额外的数据，通常配合UI场景的要求提供所需数据
@export var addition_data: Dictionary
