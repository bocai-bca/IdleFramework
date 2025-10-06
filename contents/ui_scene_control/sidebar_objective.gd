extends PanelContainer
class_name UIScene_Control_SideBarObjective

## 类场景封包
const CPS: PackedScene = preload("res://contents/ui_scene_control/sidebar_objective.tscn")

@onready var n_button: Button = $Button as Button
@onready var n_icon: TextureRect = $HBC/Icon as TextureRect
@onready var n_text: Label = $HBC/Text as Label
