import ('DotRPG.Scripting', 'DotRPG.Objects')

event_counts = {default=1,treetalk=7}

_manifest = {
    id="testDialog",
    useWith = "example/topdown",
}

shake_count = 0

function loop(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if obj:GetVelocityDerivative("whiterect") < -32 then
            camera:Shake(10 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"), 50 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"))
        end
    end
end