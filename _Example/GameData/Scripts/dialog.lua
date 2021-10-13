import ('DotRPG.Scripting', 'DotRPG.Objects')

event_counts = {"whiterecttalk":7}

function loop(objID, eventID, frameTime, totalTime)
    if objID == 0 then
    if eventID == 0 then
        dialog.Text = "You ran across white rectangular object.\nSo what?"
    elseif eventID == 1 then
        dialog.Text = "You should better go elsewhere."
        scene.ExitDialog = true
    elseif eventID == 2 then
        dialog.Text = "A white rectangular object."
        scene.ExitDialog = true
    elseif eventID == 3 then
        dialog.Text = "You've never seen object that is more white\nand rectangular than this one."
        scene.ExitDialog = true
    elseif eventID == 4 then
        dialog.Text = "Don't you have better things to do?"
        scene.ExitDialog = true
    elseif eventID == 5 then
        dialog.Text = "You ran across white rect"
        scene.AutoScroll = true
    elseif eventID == 6 then
        dialog.Text = "COME ON, QUIT IT ALREADY!"
        scene.ExitDialog = true
    end
end
end