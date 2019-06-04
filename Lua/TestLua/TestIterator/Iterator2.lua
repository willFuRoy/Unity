function mypairs1(count, curr)
	if curr < count
		then curr = curr + 1 
	return curr, curr * curr	
end
end

for i,v in mypairs1,3,0
do
	print(i,v)
end