t4 = {"key1", "key2"}

function ElementIterator(collection)
	local index = 0
	local count = #collection
	return function ()
		index = index + 1	
		if index <= count
		then				
			return collection[index]
		end
	end 
end

for element in ElementIterator(t4)
do
	print(element)	
end
