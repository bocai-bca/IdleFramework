## 放置引擎-物品注册表对象，用于在游戏资源中注册物品
extends Resource
class_name IdleFramework_ItemRegistryObject

## ID，该物品的唯一标识ID
## 此项留空将使该物品在注册时失效
@export var id: StringName

## 名称翻译标识符，即翻译表中该物品名称的键
## 此项留空将使该物品没有名称
@export var name_key: StringName

## 图标，该物品显示在GUI上的图标，将基于引擎设置自动生成Mipmap
@export var icon: Texture2D

## 描述翻译标识符，即翻译表中该物品描述的键
@export var lore_key: StringName
