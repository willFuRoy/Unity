t1 = {"key1", "key2"}

for i,v in ipairs (t1) do
	print(i,v)
end

function mypairs1(count, curr)
	if(curr < count)
		then curr = curr + 1 
	end
	return curr, curr * curr	
end

for i,v in mypairs1,3,0
do
	print(i,v)
end