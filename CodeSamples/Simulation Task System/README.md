<h1 align="middle">Simulation tasks system</h1>

<br><b>Step</b> - branch class responsible for managing sub-blocks (other steps or tasks). 
<br><b>Task</b> - leaf, base class for classes containing information about specific tasks; 
	manages its:
<br>		<i>add-ons</i> (events that lives with a task), 
<br>		<i>requirements</i> (conditions required to complete task),
<br>		<i>breakdown points</i> (containing definitions for things that can be broken and therefore reported by player)
<br>		<i>simulation events</i> (events that lives independently from tasks)
<br>		<i>fail conditions</i> (defining when the task is completed with negative result)
<br><b>FlowController</b> - classes containing logic for how the sub-blocks should be handled (in what order, parallel/sequential, are all sub blocks completion needed to proceed with next step)
