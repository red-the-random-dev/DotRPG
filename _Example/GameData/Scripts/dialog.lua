import ('DotRPG.Scripting', 'DotRPG.Objects')

event_count = 2

function loop(eventID, frameTime, totalTime)
    if eventID == 0 then
        dialog.Text = "You ran across white rectangular object.\nSo what?"
    else if eventID == 1 then
        dialog.Text = "You should better go elsewhere."
        scene.ExitDialog = true
    else if eventID == 2 then
        dialog.Text = "A white rectangular object."
        scene.ExitDialog = true
    end
end
end