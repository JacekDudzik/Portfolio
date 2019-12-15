<h1 align="middle">Simulation tasks system</h1>

Step - branch class responsible for managing sub-blocks (other steps or tasks). 
Task - leaf, base class for classes containing information about specific tasks; 
	manages its:
		add-ons (events that lives with a task), 
		requirements (conditions required to complete task),
		breakdown points (containing definitions for things that can be broken and therefore reported by player)
		simulation events (events that lives independently from tasks)
		fail conditions (defining when the task is completed with negative result)
FlowController - classes containing logic for how the sub-blocks should be handled (in what order, parallel/sequential, are all sub blocks completion needed to proceed with next step)