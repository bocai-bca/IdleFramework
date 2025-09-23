## 放置引擎-主节点，本脚本需附着在一个Node节点上，该节点作为项目的主场景根节点
extends Node
class_name IdleFramework_MotherNode

## UI场景，指定一个PackedScene作为用于呈现画面和处理玩家输入的场景，该场景需要自行访问核心来获取数据。
## 在不知道如何实现一个功能完备的自定义UI场景之前，建议使用原厂UI场景
@export var ui_scene: PackedScene
## 游戏资源，指定一个IdleFramework_GameResource，作为定义游戏内容的数据源
@export var game_resource: IdleFramework_GameResource

func _ready() -> void:
	pass
