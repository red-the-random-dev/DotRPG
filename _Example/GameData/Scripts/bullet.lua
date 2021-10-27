event_counts = {default=1}

_manifest = {
    id="kickable",
    useWith = "example/topdown",
}

lifetime = 0

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if lifetime == 0 then
            obj:ApplyForce(this, 256*15, totalTime % 1000 / 1000 * 2 * 3.14)
        end
        lifetime = lifetime + frameTime
        if lifetime > 1000 then
            obj:DestroyObject(this)
        end
    end
end