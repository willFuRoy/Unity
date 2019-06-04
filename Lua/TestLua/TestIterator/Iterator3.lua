t3 = {"key1", nil , "key3"}

-- ipairs碰到nil则中断退出，不再执行且不会执行此次循环
for i,v in ipairs(t3) do
	print('ipairs')	
	print(i,v)
end

-- pairs碰到nil不会退出，将继续执行,但不会执行此次循环 
for k,v in pairs(t3) do
	print('pairs')		
--	print(k,v)
end