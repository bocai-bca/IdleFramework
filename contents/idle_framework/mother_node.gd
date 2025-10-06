## 放置引擎-主节点，本脚本需附着在一个Node节点上，该节点作为项目的主场景根节点
extends Node
class_name IdleFramework_MotherNode

## UI场景，指定一个PackedScene作为用于呈现画面和处理玩家输入的场景，该场景需要自行访问核心来获取数据。
## 若不知道如何实现一个功能完备的自定义UI场景，建议使用原厂UI场景
@export var ui_scene: PackedScene
## 游戏资源，指定一个IdleFramework_GameResource，作为定义游戏内容的数据源
@export var game_resource: IdleFramework_GameResource

## 所有空间实例
var spaces: Dictionary[StringName, IdleFramework_SpaceInstance]

func _ready() -> void:
	start()

## 启动游戏资源
func start() -> void:
	if (ui_scene == null):
		push_warning("IdleFramework_MotherNode: UI Scene hasn't been set. This IdleFramework instance will run without UI Scene.")
	if (game_resource == null):
		push_error("IdleFramework_MotherNode: Game Resource must be set, but current is not. So this IdleFramework instance will be deleted.")
		queue_free()
		return
	## 00实例化空间
	if (game_resource.space_registry == null):
		push_error("IdleFramework_MotherNode: Space Registry hasn't been set. This IdleFramework instance will be deleted.")
		queue_free()
		return
	for space_id in game_resource.space_registry.keys():
		spaces[space_id] = game_resource.space_registry[space_id].make_instance()
	## /00
	## 01放置UI场景
	add_child(ui_scene.instantiate())
	## /01

## 重新载入游戏资源
func reload() -> void:
	for child in get_children():
		child.queue_free()
	spaces.clear()
	start()

## 获取空间实例，需要传入一个空间的ID，如果未能找到持有给定ID的空间实例将推送一个错误并返回null
func get_space(id: StringName) -> IdleFramework_SpaceInstance:
	if (spaces.has(id)):
		return spaces[id]
	push_error("IdleFramework_MotherNode: Failed to get space which id is \"" + id + "\".")
	return null

## 获取所有空间实例的ID，如果没有空间实例将返回空数组
func get_spaces_id() -> PackedStringArray:
	if (spaces == null):
		return []
	return spaces.keys()

## 获取所有空间实例
func get_spaces() -> Array[IdleFramework_SpaceInstance]:
	if (spaces == null):
		return []
	return spaces.values()
