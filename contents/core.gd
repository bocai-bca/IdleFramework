extends Node
class_name Core
## 核心

## 信号-提交主场景，当核心准备好接收主场景时，将发出此信号，请在自定义脚本中订阅此信号并在接收时向本类的main_scene赋值
signal submit_main_scene()
## 信号-提交资源，当核心准备好接收游戏资源时，将发出此信号，请在自定义脚本中订阅此信号并在接收时向本类的game_resource赋值
signal submit_game_resource()

## 主场景
static var main_scene: PackedScene
## 游戏资源
static var game_resource: IdleFramework.GameResource

## 主场景实例节点
var main_scene_node: Node

func _ready() -> void:
	emit_signal(&"submit_game_resource")
	emit_signal(&"submit_main_scene")
