function fucntion3()
	local i = 1
	return function()
		print(i)
		i = i + 1
	end
end

f1 = fucntion3()
f1()
f1()
f1()

f2 = fucntion3()
f2()
f2()
f2()

f1 = nil
f2 = nil