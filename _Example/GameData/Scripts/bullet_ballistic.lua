event_counts = {default=1}

_manifest = {
    id="kickable",
    useWith = "example/topdown",
}

lifetime = 0

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        obj:ApplyForce(this, 64, obj:RadFromDeg(90))
        lifetime = lifetime + frameTime
        if lifetime > 2000 then
            obj:DestroyObject(this)
        end
    end
end