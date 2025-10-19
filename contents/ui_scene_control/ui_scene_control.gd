## UI场景-控件
extends PanelContainer
class_name UIScene_Control

## 协议：
##

## 主节点
var mother_node: IdleFramework_MotherNode

@onready var n_sidebar_vbc: VBoxContainer = $VBC_TitleMainSeparat/PC_Main/HSC/SC_SideBar/VBC as VBoxContainer

func _enter_tree() -> void:
	print("UIScene_Control: Enter tree.")
	var parent: Node = get_parent()
	if (parent is IdleFramework_MotherNode):
		mother_node = parent
	else:
		push_error("UIScene_Control: Parent node is not IdleFramework_MotherNode. This node is going to be deleted.")

func _ready() -> void:
	## 00创建侧边栏节点
	print("UIScene_Control: Start to create sidebar nodes.")
	print("UIScene_Control: Foreaching SpaceInstances.")
	for space_instance in mother_node.get_spaces():
		if (space_instance.used_for == IdleFramework_Space.UsedFor.SIDE_BAR):
			var new_sidebar_obj: UIScene_Control_SideBarObjective = UIScene_Control_SideBarObjective.CPS.instantiate() as UIScene_Control_SideBarObjective
			n_sidebar_vbc.add_child(new_sidebar_obj)
			new_sidebar_obj.n_icon.texture = space_instance.icon
			new_sidebar_obj.n_text.text = space_instance.name_key
	## /00

func _process(delta: float) -> void:
	pass

## 本方法用于与IdleFramework内核交互进行数据更新，可在设置中通过设置物理帧速率来控制数据更新频率
func _physics_process(delta: float) -> void:
	pass
